using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDLAsserts.asserts
{
    internal interface Asserts
    {
        public string getName();
        public void Assert(bool flag);
    }
}
