using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Value
    {
        public static Value TRUE {
            get { return new Value(true, ValueType.BOOLEAN); }
            private set { }
        }
        public static Value FALSE {
            get { return new Value(false, ValueType.BOOLEAN); }
            private set { }
        }
        public static Value NULL {
            get { return new Value(null, ValueType.NULL); }
            private set { }
        }

        public static Dictionary<string, Func<Value[], Value, Value>> nativeCalls = new Dictionary<string, Func<Value[], Value, Value>>();

        /// <summary>
        /// 
        /// </summary>
        public ValueType type { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public Value(object value, ValueType type) {
            this.type = type;
            this.value = value;
            if (type != ValueType.NATIVE_CALL) assignNativeCalls();
        }

        protected virtual void assignNativeCalls() {

        }

        public virtual Value clone() {
            return new Value(value, type);
        }

        public virtual object getValue() {
            return value;
        }

        public virtual T getValue<T>() {
            return (T)value;
        }

        public virtual string str() {
            return ToString();
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

        public bool isType(ValueType types) {
            return isType((int) types);
        }

        public Type getValueSystemType() {
            return value.GetType();
        }

        public override string ToString() {
            return (value != null ? value.ToString() : (type == ValueType.NULL ? "<NULL>" : "undefined"));
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
                case ValueType.REFERENCE: return typeof(Reference);
                case ValueType.OBQECT: return typeof(Obqect);
                case ValueType.QLASS: return typeof(Qlass);
                case ValueType.FUNQTION: return typeof(Funqtion);
                default: return null;
            }
        }
    }

    public class Value<T> : Value
    {
        public new T value {
            get { return (T)base.value; }
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
        BOOLEAN = 64,
        STRING = 128,
        ARRAY = 256,
        OBQECT = 512,
        FUNQTION = 1024,
        NATIVE_CALL = 1337,
        QLASS = 2048,
        REFERENCE = 4096,
        UNDEFINED = 16384,
        NUMBER = INTEGER + DECIMAL,
        PRIMITIVE = NUMBER + BOOLEAN + STRING,
        CONTEXT = QLASS + FUNQTION + OBQECT,
        ANY_VALUE = PRIMITIVE + ARRAY + CONTEXT   
    }
}
