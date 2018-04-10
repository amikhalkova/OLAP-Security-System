using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    internal class SelectKey
    {
        public string DimName { get; private set; }

        public byte[] Key { get; private set; }

        public byte[] IV { get; private set; }

        public SelectKey(string dimName)
        {
            this.DimName = dimName;
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.GenerateIV();
                this.IV = aes.IV;
                aes.GenerateKey();
                this.Key = aes.Key;
            }
        }
    }
}
