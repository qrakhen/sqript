using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    /***
     
     *~ a <~ [ 3, '5', { n <~ 't'}]; 
     
     *<t1>~ t <~ [ val(t1), ... ];    
         
     ***/
    internal class Array : Collection<int>
    {
        public const string
            CHAR_OPEN = "[",
            CHAR_CLOSE = "]";

        public Array(Dictionary<int, Reference> value) : base(ValueType.Array, value) { }
        public Array() : base(ValueType.Array, new Dictionary<int, Reference>()) { }

        public virtual void add(Reference item) {
            int free = 0;
            do {
                if (get(free) == null) break;
                else free++;
            } while (true);
            set(free, item);
        }

        public void add(Value item) {
            add(new Reference(item));
        }

        public override void set(int key, Reference item) {
            if (key < 0) add(item);
            else base.set(key, item);
        }

        public override string ToString() {
            string r = base.ToString();
            r = "[" + r.Substring(1, r.Length - 2) + "]";
            return r;
        }
    }
}
