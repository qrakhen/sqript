using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public abstract class Value<T>
    {
        public ValueType type { get; protected set; }
        public T value { get; protected set; }

        public Value(ValueType type, T value) {
            this.type = type;
            this.value = value;
        }

        public virtual T getValue() {
            return value;
        }

        public virtual void setValue(T value) {
            this.value = value;
        }

        public virtual void setValue(T value, ValueType type) {
            this.value = value;
            this.type = type;
        }

        public bool isType(int types) {
            return (((int)type & types) > 0);
        }

        public static Type getSystemType(ValueType type) {
            switch (type) {
                case ValueType.KEYWORD: return typeof(Keyword);
                case ValueType.OPERATOR: return typeof(Operator);
                case ValueType.STRUCTURE: return typeof(string);
                case ValueType.INTEGER: return typeof(int);
                case ValueType.DECIMAL: return typeof(double);
                case ValueType.NUMBER: return typeof(decimal);
                case ValueType.STRING: return typeof(string);
                case ValueType.BOOLEAN: return typeof(bool);
                case ValueType.OBQECT: return typeof(Object);
                case ValueType.REFERENCE: return typeof(Reference);
                case ValueType.FUNCTION: return typeof(Function);
                default: return null;
            }
        }

        public override string ToString() {
            return (value != null ? value.ToString() : "nyd");
        }

        public virtual string toDebug() {
            return "(" + type.ToString() + ") " + (value != null ? value.ToString() : "");
        }
    }

    public class Value : Value<object>
    {
        public static Value NULL {
            get {
                return new Value(ValueType.NULL, null);
            }
            private set { }
        }

        public Value(ValueType type, object value) : base(type, value) { }

        public override object getValue() {
            return value;
        }

        public virtual T getValue<T>() {
            return (T)value;
        }

        public override void setValue(object value) {
            if (type == ValueType.NULL) return;
            this.value = value;
        }

        public override void setValue(object value, ValueType type) {
            if (type == ValueType.NULL) return;
            this.value = value;
            this.type = type;
        }

        public Type getValueSystemType() {
            return value.GetType();
        }
    }

    public enum ValueType
    {
        NULL = 0,
        KEYWORD = 1,
        OPERATOR = 2,
        STRUCTURE = 4,
        IDENTIFIER = 8,
        INTEGER = 16,
        DECIMAL = 32,
        NUMBER = 48,
        STRING = 128,
        BOOLEAN = 256,
        OBQECT = 512,
        ARRAY = 1024,
        REFERENCE = 2048,
        FUNCTION = 4096,
        UNDEFINED = 8192
    }
}
