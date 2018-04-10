using System;
using System.Numerics;
using LightOPE;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class LightOpeTests
    {
        [TestMethod]
        public void LightOpeEncryptionTest()
        {
            var lightOpe = new OpeSystem(100, 567);
            var c = lightOpe.Encrypt(50);
            var m = lightOpe.Decrypt(c);
            Assert.AreEqual(50, m);
        }

        [TestMethod]
        public void LightOpeEncryptionWithComparing()
        {
            var lightOpe = new OpeSystem(100, 567);
            var c1 = lightOpe.Encrypt(50);
            var c2 = lightOpe.Encrypt(95);
            Assert.IsTrue(c2 > c1);
        }
    }
}
