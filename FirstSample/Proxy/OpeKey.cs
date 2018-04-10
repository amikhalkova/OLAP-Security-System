using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightOPE;

namespace Proxy
{
    internal class OpeKey
    {
        public string DimName { get; private set; }

        public SelectKey SelectKey { get; private set; }

        public LightOPE.OpeSystem OpeSystem { get; private set; }

        public OpeKey(string dimName)
        {
            this.DimName = dimName;
            this.SelectKey = new SelectKey(dimName);
            this.OpeSystem = new OpeSystem(500, (new Random()).Next(1, 123));
        }
    }
}
