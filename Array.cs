using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Array : Value<Dictionary<int, Value>>
    {
        public Array(Dictionary<int, Value> value) : base(ValueType.ARRAY, value) { }
        public Array() : base(ValueType.ARRAY, new Dictionary<int, Value>()) { }

        
    }
}
