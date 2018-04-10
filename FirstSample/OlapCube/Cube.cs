using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OlapCube
{
    public class Cube : ICube
    {
        /// <summary>
        /// Набор измерений, который хранится в кубе
        /// </summary>
        public Dictionary<string, List<string>> Dimensions { get; private set; }

        /// <summary>
        /// Отображение названий измерений из строки в индексы
        /// </summary>
        private Dictionary<string, int> reflectionDimToIntIndexes;

        /// <summary>
        /// Непосредственно многомерный массив
        /// </summary>
        private Array cube;

        /// <summary>
        /// конструктор куба, здесь просто происходит инициализация
        /// </summary>
        public Cube()
        {
            this.Dimensions = new Dictionary<string, List<string>>();
            this.reflectionDimToIntIndexes = new Dictionary<string, int>();
        }

        /// <summary>
        /// Добавление измерения в куб
        /// </summary>
        /// <typeparam name="T">Тип измерения, но у нас всё будет в виде строки, так что можно
        /// забить на это</typeparam>
        /// <param name="dimName">Название измерения</param>
        /// <param name="items">Объекты для этого измерения</param>
        public void AddDimension<T>(string dimName, List<string> items)
        {
            //// проверяем, есть или нет в кубе данного измерения
            if (!this.Dimensions.ContainsKey(dimName))
            {
                //// добавляем измерение
                this.Dimensions.Add(dimName, items);
                //// добавляем отображение название измерения и индекс, по которому он значится в массиве
                this.reflectionDimToIntIndexes.Add(dimName, reflectionDimToIntIndexes.Count);
                //// вычисляем, сколько элементов в это измерении
                var arrayLength = this.Dimensions.Select(d => d.Value.Count).ToArray();
                //// Расширяем куб на измерение и его размерность
                this.cube = Array.CreateInstance(typeof(string), arrayLength);
                return;
            }

            throw new ArgumentOutOfRangeException("Данное измерение уже существует");
        }

        /// <summary>
        /// Добавляем элемент в куб
        /// </summary>
        /// <param name="dimItems">Названия измерение + его элемент</param>
        /// <param name="item">Значение добавляемой ячейки</param>
        public void AddItem(Dictionary<string, string> dimItems, string item)
        {
            //// получаем индексы измерений
            var indexes = this.GetIndexes(dimItems);
            //// Добавляем элемент в куб
            this.cube.SetValue(item, indexes);
        }

        /// <summary>
        /// Получение элемента
        /// </summary>
        /// <param name="dimItems">Измерение + элемент</param>
        /// <returns></returns>
        public string GetItem(Dictionary<string, string> dimItems)
        {
            var indexes = this.GetIndexes(dimItems);
            return (string)this.cube.GetValue(indexes);
        }

        /// <summary>
        /// Сумма двух элементов
        /// </summary>
        /// <param name="dimItem1">Измерения для элемента 1</param>
        /// <param name="dimItem2">измерения для элемента 2</param>
        /// <param name="isDecrypted">Зашифровано или нет (по-разному считается)</param>
        /// <returns></returns>
        public string GetSumOfItems(Dictionary<string, string> dimItem1, Dictionary<string, string> dimItem2, bool isDecrypted = false)
        {
            ////Переводим элементы в инт
            var item1 = BigInteger.Parse(this.GetItem(dimItem1));
            var item2 = BigInteger.Parse(this.GetItem(dimItem2));

            ////Если зашифровано - складываем так
            if (isDecrypted)
            {
                return BigInteger.Multiply(item1, item2).ToString();
            }
            else
            {
                return (item1 + item2).ToString();
            }
        }

        /// <summary>
        /// Получение большего элемента
        /// </summary>
        /// <param name="dimItem1">Данные для 1 элемента</param>
        /// <param name="dimItem2">Данные для 2 элемента</param>
        /// <param name="isDecrypted">Зашифрован или нет, сравнение будет по разному</param>
        /// <param name="iv">Для расшифровки симметричного уровня</param>
        /// <param name="key">Для расшифровки симметричного уровня</param>
        /// <returns></returns>
        public string GetGreaterItem(Dictionary<string, string> dimItem1, Dictionary<string, string> dimItem2, bool isDecrypted = false, byte[] iv = null, byte[] key = null)
        {
            //// получаем элементы по измерениям
            var item1 = this.GetItem(dimItem1);
            var item2 = this.GetItem(dimItem2);

            if (!isDecrypted)
            {
                return Convert.ToInt32(item1) > Convert.ToInt32(item2)
                    ? Convert.ToString(item1) : Convert.ToString(item2);
            }
            else
            {
                //// если зашифровано, сравниваем так
                var opeItem1 = BigInteger.Parse(this.AesDecrypt(item1, iv, key));
                var opeItem2 = BigInteger.Parse(this.AesDecrypt(item2, iv, key));
                return opeItem1 > opeItem2 
                    ? opeItem1.ToString()
                    : opeItem2.ToString();
            }
        }

       /// <summary>
       /// Расшифрование AES
       /// </summary>
       /// <param name="encStr">Строка в base64</param>
       /// <param name="iv">IV</param>
       /// <param name="key">Ключ</param>
       /// <returns></returns>
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

        /// <summary>
        /// Получение индексов для измерений по их названию
        /// </summary>
        /// <param name="dimItems"></param>
        /// <returns></returns>
        private int[] GetIndexes(Dictionary<string, string> dimItems)
        {
            var indexes = new int[this.Dimensions.Count];
            foreach (var dim in this.Dimensions)
            {
                if (!dimItems.ContainsKey(dim.Key))
                {
                    //// TODO: если надо будет прикрутить агрегирование, то эту проверку надо будет убрать
                    throw new ArgumentOutOfRangeException("Не указан один из индексов");
                }

                indexes[reflectionDimToIntIndexes[dim.Key]]
                    = dim.Value.FindIndex(x => x == dimItems[dim.Key]);
            }

            return indexes;
        }
    }
}
