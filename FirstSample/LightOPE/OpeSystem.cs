using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LightOPE
{
    public class OpeSystem
    {
        private int[] y;
        private BigInteger[] z;
        private BigInteger[] S;

        public OpeSystem(int size, int key)
        {
            this.y = new int[size];
            var r = new Random(key);
            for (int i = 0; i < size; i++)
            {
                this.y[i] = r.Next(1, size);
            }

            this.z = new BigInteger[size];
            this.S = new BigInteger[size];
            this.S[0] = this.y[0];
            this.z[0] = this.y[0];
            for (int i = 1; i < size; i++)
            {
                var a = BigInteger.Abs(size - S[i]);
                z[i] = y[i] * a / size + 1;
                S[i] = S[i - 1] + z[i];
            }
        }

        public BigInteger Encrypt(int m)
        {
            return S[m];
        }

        public int Decrypt(BigInteger c)
        {
            return S.ToList().FindIndex(x => x == c);
        }
    }
}
