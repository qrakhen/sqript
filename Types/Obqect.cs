using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Obqect : Collection<string>
    {
        public Obqect(Dictionary<string, Value> value) : base(ValueType.OBQECT, value) {
        }

        public Obqect() : base(ValueType.OBQECT, new Dictionary<string, Value>()) {
        }
    }
}
