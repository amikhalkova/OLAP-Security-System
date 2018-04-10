using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxy;

namespace UnitTests
{
    [TestClass]
    public class ProxyCubeFunctionalTests
    {
        [TestMethod]
        public void ProxyCubeFunctionalTest()
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

            var dimItems = new Dictionary<string, string>();
            dimItems.Add("Month", "September");
            dimItems.Add("Country", "USA");
            dimItems.Add("Product", "Product_Bear");
            cube.AddItem(dimItems, "Ururur");

            dimItems.Add("NotExistingDim", "sdf");
            try
            {
                cube.AddItem(dimItems, "123");
            }
            catch (Exception ex)
            {
                if (!(ex is KeyNotFoundException))
                {
                    throw;
                }
            }

            dimItems.Remove("NotExistingDim");
            var dimItems2 = new Dictionary<string, string>();
            dimItems2.Add("Month", "March");
            dimItems2.Add("Country", "Russia");
            dimItems2.Add("Product", "Product_Bear");
            cube.AddItem(dimItems2, "20");

            try
            {
               cube.GetSumOfItems(dimItems, dimItems2);
            }
            catch (Exception ex)
            {
                if (!(ex is ArgumentNullException))
                {
                    throw;
                }
            }

            try
            {
                cube.GetGreaterItem(dimItems, dimItems2);
            }
            catch (Exception ex)
            {
                if (!(ex is ArgumentNullException))
                {
                    throw;
                }
            }

            dimItems["Country"] = "Russia";
            cube.AddItem(dimItems, "123");

            cube.GetSumOfItems(dimItems, dimItems2);
            cube.GetGreaterItem(dimItems, dimItems2);
        }
    }
}
