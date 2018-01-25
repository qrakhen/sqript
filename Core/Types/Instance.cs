using System.Collections.Generic;

namespace Qrakhen.Sqript
{
    internal class Instance : Obqect
    {
        public Qlass constructor { get; protected set; }

        public Instance(Qlass constructor, Qontext parent, Dictionary<string, Reference> value) : base(parent, value) {
            this.constructor = constructor;
        }
    }
}
