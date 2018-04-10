using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paillier;

namespace UnitTests
{
    [TestClass]
    public class PailierTests
    {
        [TestMethod]
        public void PailierEncryptionNumTest()
        {
            var p = new PailierSystem();
            while (p.n == null || p.n < 20)
            {
                p.GenerateKey();
            }

            var m = 13;
            var c = p.Encrypt(m);
            var d = p.Decrypt(c);
            Assert.AreEqual(m, d);
        }

        [TestMethod]
        public void PailierEncryptionSumTest()
        {
            var p = new PailierSystem();
            while (p.n == null || p.n < 20)
            {
                p.GenerateKey();
            }

            var m1 = 13;
            var m2 = 5;
            var c1 = p.Encrypt(m1);
            var c2 = p.Encrypt(m2);

            var d = p.Decrypt(BigInteger.Multiply(c1, c2) %  BigInteger.Multiply(p.n, p.n));
            Assert.AreEqual(18, d);
        }
    }
}
