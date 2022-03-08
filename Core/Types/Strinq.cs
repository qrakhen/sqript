using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{

	internal class Strinq : QValue<string>
	{

		protected Encoding _encoding;

		static Strinq() {
			Native.Define(
				ValueType.String,
				"indexOf",
				NativeIndexOf);

			Native.Define(
				ValueType.String,
				"length",
				NativeLength);
		}

		public Strinq(string value, ValueType type, Encoding encoding = Encoding.UTF8) : base(type, value) {
			this._encoding = encoding;
		}


		public static QValue NativeIndexOf(QValue target, QValue[] parameters) {
			if (parameters.Length < 1)
				return Int(-1);
			string subString = parameters[0].Str();
			return target.Value != null ? Int(target.GetValue<string>().IndexOf(subString)) : Int(-1);
		}

		public static QValue NativeLength(QValue target, QValue[] parameters) {
			return target.Value != null ? Int(target.GetValue<string>().Length) : Int(0);
		}


		public enum Encoding {
			UTF8,
			UTF16,
			UTF32,
			ASCII
		}
	}
}