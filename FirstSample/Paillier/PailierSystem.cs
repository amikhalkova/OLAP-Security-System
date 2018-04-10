using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Paillier
{
    public class PailierSystem
    {
        public BigInteger n { get; private set; }

        public BigInteger g { get; private set; }

        public BigInteger lambda { get; private set; }

        public BigInteger mu { get; private set; }

        public PailierSystem()
        {
        }

        public PailierSystem(BigInteger n, BigInteger g, BigInteger lambda, BigInteger mu)
        {
            this.n = n;
            this.g = g;
            this.lambda = lambda;
            this.mu = mu;
        }

        public void GenerateKey()
        {
            var p = PailierHelper.GeneratePrimeNumber();
            var q = PailierHelper.GeneratePrimeNumber();
            while (PailierHelper.GCD(BigInteger.Multiply(p, q), BigInteger.Multiply(p - 1, q - 1)) != 1 || p == q)
            {
                p = PailierHelper.GeneratePrimeNumber();
                q = PailierHelper.GeneratePrimeNumber();
            }

            this.n = BigInteger.Multiply(p, q);
            this.lambda = PailierHelper.LCM(p - 1, q - 1);

            this.mu = -1;
            while (this.mu == -1)
            {
                this.g = PailierHelper.GetRandomFromMultRing(BigInteger.Multiply(n, n));

                BigInteger x = BigInteger.ModPow(g, lambda, BigInteger.Multiply(n, n));
                this.mu = PailierHelper.GetInverseElement(BigInteger.Divide(x - 1, n), BigInteger.Multiply(n, n)) % n;
            }
        }

        public BigInteger Encrypt(BigInteger m)
        {
            if (this.n == null || this.g == null || this.lambda == null || this.mu == null)
            {
                throw new ArgumentOutOfRangeException("Сгенерируйте ключи, преждем чем шифровать!");
            }

            if (m > n)
            {
                throw new ArgumentOutOfRangeException("m > n, необходимо заново сгенерировать ключи");
            }

            var r = PailierHelper.GetRandomFromMultRing(this.n);
            var sqrn = BigInteger.Multiply(this.n, this.n);
            var a = BigInteger.ModPow(this.g, m, sqrn);
            var b = BigInteger.ModPow(r, this.n, sqrn);
            var c = BigInteger.Multiply(a, b) % sqrn;

            return c;
        }

        public BigInteger Decrypt(BigInteger c)
        {
            var sqrn = BigInteger.Multiply(this.n, this.n);
            var x = BigInteger.ModPow(c, this.lambda, sqrn);
            var d = BigInteger.Multiply(BigInteger.Divide(x - 1, this.n), this.mu) % n;
            return d;
        }
    }
}
