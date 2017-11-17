using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Obqect : Value<Dictionary<string, Obqect>>
    {

        public Obqect(Dictionary<string, Obqect> value) : base(ValueType.OBQECT, value) {

        }

        public Obqect() : base(ValueType.OBQECT, new Dictionary<string, Obqect>()) { 
       
        }
    }
}
