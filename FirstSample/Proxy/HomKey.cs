using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    internal class HomKey
    {
        public string DimName { get; private set; }

        public Paillier.PailierSystem PailierSystem { get; private set; }

        public HomKey(string dimName)
        {
            this.DimName = dimName;
            this.PailierSystem = new Paillier.PailierSystem();
        }
    }
}
