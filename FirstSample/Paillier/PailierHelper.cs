using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Paillier
{
    public static class PailierHelper
    {
        public static BigInteger GetRandomFromMultRing(BigInteger mod)
        {
            var rand = new Random();
            BigInteger a = rand.Next() % mod;
            while (GCD(a, mod) != 1)
            {
                a = rand.Next() % mod;
            }

            return a;
        }

        public static int GCD(BigInteger a, BigInteger b)
        {
            var a1 = a;
            var b1 = b;
            BigInteger rem = 0;
            while (a1 != 0 && b1 != 0)
            {
                if (a1 > b1)
                {
                    a1 %= b1;
                }
                else
                {
                    b1 %= a1;
                }
            }

            return a1 == 0 ? (int)b1 : (int)a1;
        }

        public static int GeneratePrimeNumber()
        {
            var r = new Random();
            var a = Math.Abs(r.Next(300));
            while (!IsPrime(a) || a <= 1)
            {
                a = Math.Abs(r.Next(100));
            }

            return a;
        }

        public static bool IsPrime(int a)
        {
            for (int i = 2; i <= Math.Sqrt(a); i++)
            {
                if (a % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static BigInteger LCM(int a, int b)
        {
            return BigInteger.Divide(BigInteger.Multiply(a, b), GCD(a, b));
        }

        public static BigInteger GetInverseElement(BigInteger a, BigInteger n)
        {
            BigInteger result = -1;
            for (BigInteger i = 2; i < n; i++)
            {
                var x = BigInteger.Multiply(a, i) % n;
                if (x == 1)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }
    }
}
