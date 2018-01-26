using System.Collections.Generic;

namespace Qrakhen.Sqript
{
    internal class Qroup : Qontext
    {
        public Dictionary<string, Property> qlasses;

        /// <summary>
        /// Qlass.value is a Dictionary with all STATIC references, so a qlass without any static properties won't have anything inside its value.
        /// </summary>
        /// <param name="context"></param>
        public Qroup(
                Qontext parent,
                Dictionary<string, Qlass> qlasses = null,
                Dictionary<string, Reference> subQroups = null) : base(parent, ValueType.Qlass, subQroups) {
            qlasses = (qlasses ?? new Dictionary<string, Qlass>());
        }

        public Instance instantiate() {
            return null;
        }

        public class Property
        {
            public string name { get; private set; }
            public Access access { get; private set; }
            public Qlass qlass { get; private set; }

            public Property(
                    string name, 
                    Qlass qlass, 
                    Access access = Access.PUBLIC) { 
                this.name = name;
                this.qlass = qlass;
                this.access = access;
            }
        }
    }
}
