using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Obqect : Context
    {
        public Obqect(Context parent, Dictionary<string, Reference> value) : base(parent, ValueType.OBQECT, value) {}

        public Obqect(Context parent) : base(parent, ValueType.OBQECT, new Dictionary<string, Reference>()) {}
    }
}
