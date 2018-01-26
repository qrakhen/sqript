using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Value
    {
        public static readonly Value TRUE = new Value(true, ValueType.Boolean);
        public static readonly Value FALSE = new Value(false, ValueType.Boolean); 
        public static readonly Value NULL = new Value(null, ValueType.Null);
        public static readonly Value UNDEFINED = new Value(null, ValueType.Undefined);

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
            if (type != ValueType.NativeCall) assignNativeCalls();
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
            return ToString(false);
        } 

        public string ToString(bool includeType) {
            string r = (value != null ? value.ToString() : (type == ValueType.Null ? "<null>" : "undefined"));
            if (includeType) r = "<" + type + ":" + r + ">";
            return r;
        }

        public static Type getSystemType(ValueType type) {
            switch (type) {
                case ValueType.Keyword: return typeof(Keyword);
                case ValueType.Operator: return typeof(Operator);
                case ValueType.Struqture: return typeof(string);
                case ValueType.Integer: return typeof(int);
                case ValueType.Decimal: return typeof(double);
                case ValueType.Number: return typeof(decimal);
                case ValueType.String: return typeof(string);
                case ValueType.Boolean: return typeof(bool);
                case ValueType.Array: return typeof(Array);
                case ValueType.Reference: return typeof(Reference);
                case ValueType.Obqect: return typeof(Obqect);
                case ValueType.Qlass: return typeof(Qlass);
                case ValueType.Funqtion: return typeof(Funqtion);
                default: return null;
            }
        }

        public static ValueType getType(string type) {
            return (ValueType) Enum.Parse(typeof(ValueType), type);
        }
    }

    public class Readonly : Value
    {
        public new object value {
            get { return base.value; }
            set { }
        }

        public new ValueType type {
            get { return base.type; }
            set { }
        }

        public Readonly(ValueType type, object value) : base(value, type) {

        }

        public static Readonly fromValue(Value value) {
            return new Readonly(value.type, value.value);
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
        Null = 0x0000,
        Keyword = 0x0001,
        Operator = 0x0002,
        Struqture = 0x0004,
        Identifier = 0x0008,
        Integer = 0x0010,
        Decimal = 0x0020,
        Boolean = 0x0040,
        String = 0x0080,
        Array = 0x0100,
        Obqect = 0x0200,
        Funqtion = 0x0400,
        NativeCall = 1337,
        Qlass = 0x0800,
        Reference = 0x1000,
        Undefined = 0xFFFF,
        Number = Integer + Decimal,
        Primitive = Number + Boolean + String,
        Qontext = Qlass + Funqtion + Obqect,
        Any = Primitive + Array + Qontext   
    }
}
