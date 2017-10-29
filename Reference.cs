using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Reference : Value
    {
        public string name { get; protected set; }

        public Reference(string name, Type type, object value) : base(type, value) {
            this.name = name;
        }

        public override object getValue() {
            if (value == null) return null;
            else if (value.GetType() == typeof(Reference)) {
                return (value as Reference).getValue();
            }
            return value;
        }

        public override T getValue<T>() {
            if (value == null) return default(T);
            else if (value.GetType() == typeof(Reference)) {
                return (value as Reference).getValue<T>();
            }
            return (T) value;
        }

        public Reference getReference() {
            if (value == null) return null;
            else if (value.GetType() == typeof(Reference)) {
                return (value as Reference).getReference();
            }
            return this;
        }

        public override string ToString() {
            return "[" + type.ToString() + "] " + name + ": " + getValue();
        }
    }    
}
