using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Array : Collection<int>
    {
        public Array(Dictionary<int, Value> value) : base(ValueType.ARRAY, value) { }
        public Array() : base(ValueType.ARRAY, new Dictionary<int, Value>()) { }

        public virtual void addChild(Value item) {

        }
    }
}
