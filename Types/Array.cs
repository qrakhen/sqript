using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Array : Collection<int, Reference>
    {
        public Array(Dictionary<int, Reference> value) : base(ValueType.ARRAY, value) { }
        public Array() : base(ValueType.ARRAY, new Dictionary<int, Reference>()) { }

        public virtual void add(Value item) {
            int free = 0;
            do {

            } while (get(free++) != null);
            set(free, new Reference(free.ToString(), item));
        }

        public virtual void add(Reference item) {
            int free = 0;
            do {

            } while (get(free++) != null);
            set(free, item);
        }
    }
}
