using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Obqect : Collection<string, Reference>
    {
        public Obqect(Dictionary<string, Reference> value) : base(ValueType.OBQECT, value) {}

        public Obqect() : base(ValueType.OBQECT, new Dictionary<string, Reference>()) {}

        /// <summary>
        /// Other than Context, this looks into the object rather than outside to its parents.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public virtual Reference get(object[] keys) {
            if (keys.Length < 1) throw new Exception("trying to access collection member with empty set of keys");
            Reference v = null;
            Value c = this;
            foreach (object key in keys) {
                if (c is Obqect) {
                    v = (c as Obqect).get((string)key);
                } else if (c is Array) {
                    v = (c as Array).get((int)key);
                } else throw new Exception("accessing member of non-collection value");
                c = v;
            }
            return v;
        }
    }
}
