using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OlapCube
{
    public interface ICube
    {
        void AddDimension<T>(string dimName, List<string> items);

        void AddItem(Dictionary<string, string> dimItems, string item);

        string GetItem(Dictionary<string, string> dimItems);

        string GetSumOfItems(Dictionary<string, string> dimItem1, Dictionary<string, string> dimItem2, bool isDecrypted = false);

        string GetGreaterItem(Dictionary<string, string> dimItem1, Dictionary<string, string> dimItem2, bool isDecrypted = false, byte[] iv = null, byte[] key = null);

        string AesDecrypt(string encStr, byte[] iv, byte[] key);
    }
}
