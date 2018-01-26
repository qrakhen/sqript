using System.Collections.Generic;

namespace Qrakhen.Sqript
{
    internal class Obqect : Qontext
    {
        public Obqect(Qontext parent, Dictionary<string, Reference> value) : base(parent, ValueType.Obqect, value) {
        }

        public Obqect(Qontext parent) : this(parent, new Dictionary<string, Reference>()) {}
    }
}
