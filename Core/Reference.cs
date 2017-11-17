using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Reference
    {
        public Value value { get; protected set; }
        public string name { get; protected set; }

        public Reference(string name, Value value = null) {
            this.name = name;
            this.value = (value == null ? Value.NULL : value);
        }

        public virtual void assign(Value value, bool reference = false) {
            if (reference) this.value = value;
            else {
                if (value == Value.NULL) value = new Value(value.type, value.getValue());
                else this.value.setValue(value.getValue(), value.type);
            }
        }

        public ValueType getValueType() {
            return value.type;
        }

        public virtual Value getReference() {
            return value;
        }

        public virtual object getValue() {
            return value.getValue();
        }

        public virtual T getValue<T>() {
            return value.getValue<T>();
        }

        public override string ToString() {
            return getReference().ToString();
        }

        public virtual string toDebug() {
            return name + ": " + getReference().toDebug();
        }
    }    
}
