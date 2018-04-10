using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OlapCube;

namespace UnitTests
{
    /// <summary>
    /// Summary description for CubeTest
    /// </summary>
    [TestClass]
    public class CubeTests
    {
        //// тест, проверяющий, что установлено нужное число измерений,
        ////, установлены верные значения у измерений
        [TestMethod]
        public void CreateCubeTest()
        {
            var cube = new Cube();
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

            Assert.AreEqual(cube.Dimensions.Count, 3);

            Assert.IsTrue(cube.Dimensions["Month"].Contains("September"));
            Assert.IsTrue(cube.Dimensions["Month"].Contains("March"));
            Assert.IsTrue(cube.Dimensions["Month"].Contains("December"));
            Assert.AreEqual(cube.Dimensions["Month"].Count, 3);

            Assert.IsTrue(cube.Dimensions["Country"].Contains("Russia"));
            Assert.IsTrue(cube.Dimensions["Country"].Contains("USA"));
            Assert.AreEqual(cube.Dimensions["Country"].Count, 2);

            Assert.IsTrue(cube.Dimensions["Product"].Contains("Product_Bear"));
            Assert.AreEqual(cube.Dimensions["Product"].Count, 1);

            var dimItems = new Dictionary<string, string>();
            dimItems.Add("Month", "September");
            dimItems.Add("Country", "USA");
            dimItems.Add("Product", "Product_Bear");

            cube.AddItem(dimItems, "156");
            Assert.AreEqual(cube.GetItem(dimItems), "156");
        }
    }
}
