using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal class Obqect : Qontext
    {
        public Obqect(Qontext parent, Dictionary<string, Reference> value) : base(parent, ValueType.OBQECT, value) {
            assignNativeCalls();
        }

        public Obqect(Qontext parent) : this(parent, new Dictionary<string, Reference>()) {}

        protected override void assignNativeCalls() {

        }
    }
}
