using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OlapCube;

namespace Proxy
{
    public class ProxyCube : ICube
    {
        private  Dictionary<string, string> dimensions;

        private List<SelectKey> selectKeyList;

        private SelectKey dimSelectKey;

        private SelectKey itemSelectKey;
        private HomKey itemHomKey;
        private OpeKey itemOpeKey;

        public Cube SelectCube { get; private set; }

        public Cube HomCube { get; private set; }

        public Cube OpeCube { get; private set; }

        public ProxyCube()
        {
            this.dimensions = new Dictionary<string, string>();
            this.dimSelectKey = new SelectKey("dimSelectKey");
            this.selectKeyList = new List<SelectKey>();

            this.itemSelectKey = new SelectKey("itemSelectKey");
            this.itemOpeKey = new OpeKey("itemOpeKey");
            this.itemHomKey = new HomKey("homOpeKey");
            this.itemHomKey.PailierSystem.GenerateKey();

            this.SelectCube = new Cube();
            this.HomCube = new Cube();
            this.OpeCube = new Cube();
        }

        public void AddDimension<T>(string dimName, List<string> items)
        {
            if (!this.dimensions.ContainsKey(dimName))
            {
                var encDimName = this.AesEncrypt(dimName, this.dimSelectKey.IV, this.dimSelectKey.Key);
                this.dimensions.Add(dimName, encDimName);

                var newItemsSelectKey = new SelectKey(dimName);
                this.selectKeyList.Add(newItemsSelectKey);
                var encItems = new List<string>();

                foreach (var item in items)
                {
                    var encItem = this.AesEncrypt(item, newItemsSelectKey.IV, newItemsSelectKey.Key);
                    encItems.Add(encItem);
                }

                this.SelectCube.AddDimension<string>(encDimName, encItems);
                this.HomCube.AddDimension<string>(encDimName, encItems);
                this.OpeCube.AddDimension<string>(encDimName, encItems);

                return;
            }

            throw new ArgumentOutOfRangeException("Данное измерение уже существует");
        }

        public void AddItem(Dictionary<string, string> dimItems, string item)
        {
            var selectDimItems = this.GetSelectDimItems(dimItems);

            var encSelectValue = this.AesEncrypt(
                item, this.itemSelectKey.IV, this.itemSelectKey.Key);
            this.SelectCube.AddItem(selectDimItems, encSelectValue);

            if (BigInteger.TryParse(item, out BigInteger num))
            {
                var encHomValue = this.itemHomKey.PailierSystem.Encrypt(num).ToString();
                this.HomCube.AddItem(selectDimItems, encHomValue);

                var encOpeValue = this.itemOpeKey.OpeSystem.Encrypt((int)num).ToString();
                var encSelectOpeValue = this.AesEncrypt(
                    encOpeValue, this.itemOpeKey.SelectKey.IV, this.itemOpeKey.SelectKey.Key);
                this.OpeCube.AddItem(selectDimItems, encSelectOpeValue);
            }
        }

        public string GetItem(Dictionary<string, string> dimItems)
        {
            var selectDimItems = this.GetSelectDimItems(dimItems);
            var encItem = this.SelectCube.GetItem(selectDimItems);
            var decItem = this.AesDecrypt(encItem, this.itemSelectKey.IV, this.itemSelectKey.Key);
            return decItem;
        }

        public string AesDecrypt(string encStr, byte[] iv, byte[] key)
        {
            var encBytes = Convert.FromBase64String(encStr);
            string plainText = null;
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plainText;
            //// return this.cube.AesDecrypt(encStr, key, iv);
        }

        public string GetSumOfItems(Dictionary<string, string> dimItem1, Dictionary<string, string> dimItem2, bool isDecrypted = false)
        {
            var selectedDimItem1 = this.GetSelectDimItems(dimItem1);
            var selectedDimItem2 = this.GetSelectDimItems(dimItem2);

            var n = this.itemHomKey.PailierSystem.n;
            var encSum = BigInteger.Parse(this.HomCube.GetSumOfItems(
                selectedDimItem1, selectedDimItem2, true)) % BigInteger.Multiply(n, n);

            var decValue = this.itemHomKey.PailierSystem.Decrypt(encSum) % n;

            return decValue.ToString();
        }

        public string GetGreaterItem(Dictionary<string, string> dimItem1, Dictionary<string, string> dimItem2, bool isDecrypted = false, byte[] kiv = null, byte[] key = null)
        {
            var selectedDimItem1 = this.GetSelectDimItems(dimItem1);
            var selectedDimItem2 = this.GetSelectDimItems(dimItem2);
            var opeSelectIv = this.itemOpeKey.SelectKey.IV;
            var opeSelectKey = this.itemOpeKey.SelectKey.Key;

            var encItem = this.OpeCube.GetGreaterItem(
                selectedDimItem1, selectedDimItem2, true, opeSelectIv, opeSelectKey);

            var value = this.itemOpeKey.OpeSystem.Decrypt(BigInteger.Parse(encItem));
            return value.ToString();
        }

        private Dictionary<string, string> GetSelectDimItems(Dictionary<string, string> dimItems)
        {
            var selectDimItems = new Dictionary<string, string>();
            foreach (var item in dimItems)
            {
                var key = this.dimensions[item.Key];
                var dimKey = this.selectKeyList.Where(x => x.DimName == item.Key).First();
                var value = this.AesEncrypt(item.Value, dimKey.IV, dimKey.Key);
                selectDimItems.Add(key, value);
            }

            return selectDimItems;
        }

        private string AesEncrypt(string plainText, byte[] IV, byte[] Key)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        byte[] encrypted = msEncrypt.ToArray();
                        return Convert.ToBase64String(encrypted);
                    }
                }
            }
        }

    }
}
