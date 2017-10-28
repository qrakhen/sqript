using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Value
    {
        public Type type { get; protected set; }
        public object value { get; protected set; }

        public Value(Type type, object value) {
            this.type = type;
            this.value = value;
        }

        public virtual object getValue() {
            return value;
        }

        public virtual T getValue<T>() {
            return (T) value;
        }

        public virtual void setValue(object value) {
            this.value = value;
        }

        public virtual void setValue(object value, Type type) {
            this.value = value;
            this.type = type;
        }

        public virtual void setValue<T>(T value) {
            this.value = value;
        }

        public enum Type
        {
            KEYWORD,
            OPERATOR,
            STRUCTURE,
            IDENTIFIER,
            INTEGER,
            DECIMAL,
            NUMBER,
            STRING,
            BOOLEAN,
            OBJECT,
            ARRAY,
            REFERENCE,
            FUNCTION,
            UNDEFINED
        }
    }
}
