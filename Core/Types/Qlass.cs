using System.Collections.Generic;

namespace Qrakhen.Sqript
{
    internal class Qlass : Context
    {
        public List<Property> properties { get; protected set; }

        /// <summary>
        /// Qlass.value is a Dictionary with all STATIC references, so a qlass without any static properties won't have anything inside its value.
        /// </summary>
        /// <param name="context"></param>
        public Qlass(Funqtion context, List<Property> properties) : base(context, ValueType.QLASS, new Dictionary<string, Reference>()) {
            this.properties = properties;
        }

        public Obqect instantiate() {
            return null;
        }

        public class Property
        {
            public string name { get; private set; }
            public ValueType type { get; private set; }
            public Value value { get; private set; }

            public Property(string name, ValueType type = ValueType.NULL, Value value = null) {
                this.name = name;
                this.type = type;
                this.value = value;
            }
        }
    }
}
