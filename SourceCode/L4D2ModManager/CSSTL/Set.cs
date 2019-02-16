using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSSTL
{
    class Set<T> : Dictionary<T, bool>
    {
        public void Add(T v)
        {
            base.Add(v, true);
        }

        public bool TryAdd(T v)
        {
            if (Contains(v))
                return false;
            Add(v);
            return true;
        }

        public bool Contains(T v)
        {
            return base.ContainsKey(v);
        }
    }
}
