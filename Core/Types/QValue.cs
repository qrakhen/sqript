using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript {

	public class QValue {

		public static readonly QValue True = new QValue(true, ValueType.Boolean);
		public static readonly QValue False = new QValue(false, ValueType.Boolean);
		public static readonly QValue Null = new QValue(null, ValueType.Null);
		public static readonly QValue Undefined = new QValue(null, ValueType.Undefined);

		public static Dictionary<string, Func<QValue[], QValue, QValue>> nativeCalls = new Dictionary<string, Func<QValue[], QValue, QValue>>();

		/// <summary>
		/// 
		/// </summary>
		public ValueType Type { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public object Value { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		public QValue(object value, ValueType type) {
			this.Type = type;
			this.Value = value;
		}

		public static QValue Int(int v) => new QValue(v, ValueType.Integer);
		public static QValue Decimal(decimal v) => new QValue(v, ValueType.Decimal);
		public static QValue Decimal(float v) => new QValue(Convert.ToDecimal(v), ValueType.Decimal);
		public static QValue Decimal(double v) => new QValue(Convert.ToDecimal(v), ValueType.Decimal);

		public virtual QValue Clone() {
			return new QValue(Value, Type);
		}

		public virtual object GetValue() {
			return Value;
		}

		public virtual T GetValue<T>() {
			return (T) Value;
		}

		public virtual string Str() {
			return ToString();
		}

		public virtual void SetValue(object value, ValueType type) {
			this.Value = value;
			this.Type = type;
		}

		public virtual void SetValue(QValue value) {
			SetValue(value.GetValue(), value.Type);
		}

		public bool IsType(int type) {
			return (((int) this.Type & type) > 0);
		}

		public bool IsType(ValueType type) {
			return IsType((int) type);
		}

		public bool IsNull() {
			return this.Type == ValueType.Null;
		}

		public Type GetValueSystemType() {
			return Value.GetType();
		}

		public override string ToString() {
			string r = (Value != null ? Value.ToString() : (Type == ValueType.Null ? "<null>" : "undefined"));
			return r;
		}

		public virtual string ToFullString() {
			string r = ToString();
			r = "<" + Type + ":" + r + ">";
			return r;
		}

		public static Type GetSystemType(ValueType type) {
			return type switch {
				ValueType.Keyword => typeof(Keyword),
				ValueType.Operator => typeof(Operator),
				ValueType.Struqture => typeof(string),
				ValueType.Integer => typeof(int),
				ValueType.Decimal => typeof(double),
				ValueType.Number => typeof(decimal),
				ValueType.String => typeof(string),
				ValueType.Boolean => typeof(bool),
				ValueType.Array => typeof(Array),
				ValueType.Reference => typeof(Reference),
				ValueType.Obqect => typeof(Obqect),
				ValueType.Qlass => typeof(Qlass),
				ValueType.Funqtion => typeof(Funqtion),
				_ => null,
			};
		}

		public static ValueType GetType(string type) {
			return (ValueType) Enum.Parse(typeof(ValueType), type);
		}
	}


	public class Readonly : QValue {

		public new object Value {
			get { return base.Value; }
			set { }
		}

		public new ValueType Type {
			get { return base.Type; }
			set { }
		}


		public Readonly(ValueType type, object value) : base(value, type) {

		}


		public static Readonly FromValue(QValue value) {
			return new Readonly(value.Type, value.Value);
		}
	}

	public class QValue<T> : QValue {

		public new T Value {
			get { return (T) base.Value; }
		}


		public QValue(ValueType type, T value) : base(value, type) {

		}


		public new virtual T GetValue() {
			return (T) base.GetValue();
		}

		public virtual void SetValue(T value, ValueType type) {
			base.SetValue(value, type);
		}
	}


	public enum ValueType {
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
