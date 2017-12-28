using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Value
    {
        public static Value NULL {
            get { return new Value(null, ValueType.NULL); }
            private set { }
        }

        public ValueType type { get; private set; } = ValueType.NULL;
        public object value { get; private set; } = null;

        public Value(object value, ValueType type) {
            this.type = type;
            this.value = value;
        }

        public virtual object getValue() {
            return value;
        }

        public virtual T getValue<T>() {
            return (T)value;
        }

        public virtual void setValue(object value, ValueType type) {
            this.value = value;
            this.type = type;
        }

        public virtual void setValue(Value value) {
            setValue(value.getValue(), value.type);
        }

        public bool isType(int types) {
            return (((int)type & types) > 0);
        }

        public Type getValueSystemType() {
            return value.GetType();
        }

        public override string ToString() {
            return (value != null ? value.ToString() : (type == ValueType.NULL ? "null" : "undefined"));
        }

        public virtual string toDebug() {
            return "[" + type.ToString() + "] " + this.ToString();
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
                case ValueType.ARRAY: return typeof(Array);
                case ValueType.OBQECT: return typeof(Obqect);
                case ValueType.REFERENCE: return typeof(Reference);
                case ValueType.FUNCTION: return typeof(Function);
                default: return null;
            }
        }
    }

    public class Value<T> : Value
    {
        public new T value {
            get { return (T)base.value; }
            // set { } should still be read only.
        }

        public Value(ValueType type, T value) : base(value, type) {

        }

        public new virtual T getValue() {
            return (T) base.getValue();
        }

        public virtual void setValue(T value, ValueType type) {
            base.setValue(value, type);
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
        BOOLEAN = 64,
        STRING = 128,
        CONTEXT = 256,
        OBQECT = 512,
        ARRAY = 1024,
        COLLECTION = 1792,
        REFERENCE = 2048,
        FUNCTION = 4096,
        UNDEFINED = 8192
    }
}
