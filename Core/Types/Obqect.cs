using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal class Obqect : Context
    {
        public Obqect(Context parent, Dictionary<string, Reference> value) : base(parent, ValueType.OBQECT, value) {
            assignNativeCalls();
        }

        public Obqect(Context parent) : this(parent, new Dictionary<string, Reference>()) {}

        protected void assignNativeCalls() {

        }
    }
}
