using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxy;

namespace UnitTests
{
    [TestClass]
    public class ProxyCubeTests
    {
        [TestMethod]
        public void SelectProxyTest()
        {
            var cube = new ProxyCube();
            var months = new List<string>()
            {
                "December", "March", "September"
            };

            var countries = new List<string>()
            {
                "Russia", "USA"
            };

            var products = new List<string>()
            {
                "Product_Bear"
            };

            cube.AddDimension<string>("Month", months);
            cube.AddDimension<string>("Country", countries);
            cube.AddDimension<string>("Product", products);

            Assert.AreEqual(cube.SelectCube.Dimensions.Count, 3);

            var dimItems = new Dictionary<string, string>();
            dimItems.Add("Month", "September");
            dimItems.Add("Country", "USA");
            dimItems.Add("Product", "Product_Bear");

            cube.AddItem(dimItems, "156");
            Assert.AreEqual(cube.GetItem(dimItems), "156");
        }

        [TestMethod]
        public void HomProxyTest()
        {
            var cube = new ProxyCube();
            var months = new List<string>()
            {
                "September", "March", "Dec"
            };

            var countries = new List<string>()
            {
                "12", "34"
            };

            var products = new List<string>()
            {
                "Product_Bear"
            };

            cube.AddDimension<string>("Month", months);
            cube.AddDimension<int>("Country", countries);
            cube.AddDimension<string>("Product", products);

            Assert.AreEqual(cube.HomCube.Dimensions.Count, 3);

            var dimItems1 = new Dictionary<string, string>();
            dimItems1.Add("Month", "September");
            dimItems1.Add("Country", "12");
            dimItems1.Add("Product", "Product_Bear");
            cube.AddItem(dimItems1, "156");

            var dimItems2 = new Dictionary<string, string>();
            dimItems2.Add("Month", "March");
            dimItems2.Add("Country", "12");
            dimItems2.Add("Product", "Product_Bear");
            cube.AddItem(dimItems2, "20");
            Assert.AreEqual(cube.GetSumOfItems(dimItems1, dimItems2), "176");
        }

        [TestMethod]
        public void OpeProxyTest()
        {
            var cube = new ProxyCube();
            var months = new List<string>()
            {
                "September", "March", "Dec"
            };

            var countries = new List<string>()
            {
                "12", "34"
            };

            var products = new List<string>()
            {
                "Product_Bear"
            };

            cube.AddDimension<string>("Month", months);
            cube.AddDimension<int>("Country", countries);
            cube.AddDimension<string>("Product", products);

            Assert.AreEqual(cube.OpeCube.Dimensions.Count, 3);

            var dimItems1 = new Dictionary<string, string>();
            dimItems1.Add("Month", "September");
            dimItems1.Add("Country", "12");
            dimItems1.Add("Product", "Product_Bear");
            cube.AddItem(dimItems1, "156");

            var dimItems2 = new Dictionary<string, string>();
            dimItems2.Add("Month", "March");
            dimItems2.Add("Country", "12");
            dimItems2.Add("Product", "Product_Bear");
            cube.AddItem(dimItems2, "20");
            Assert.AreEqual(cube.GetGreaterItem(dimItems1, dimItems2), "156");
        }
    }
}
