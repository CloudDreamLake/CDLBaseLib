using CDLAsserts.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDLAsserts.asserts
{
    public class RangeAssert<T> : Asserts
    {
        
        private T l, r;
        public RangeAssert(T l, T r)
        {
            this.l = l;
            this.r = r;
        }
        public void Assert(bool flag)
        {
            if (!flag)
            {
                throw new NumNotInRangeException();
            }
        }
        public void Assert(T Num) => Assert(l <= Num && Num <= r);

        public string getName()
        {
            return "Number Range Assert";
        }
    }
}
