using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript {
	internal class Strinq : Value<string> {
		public Encoding encoding { get; protected set; }

		static Strinq() {
			Native.define(
				ValueType.String,
				"indexOf",
				native_indexOf);

			Native.define(
				ValueType.String,
				"length",
				native_length);
		}

		public Strinq(string value, ValueType type, Encoding encoding = Encoding.UTF8) : base(type, value) {
			this.encoding = encoding;
		}

		public static Value native_indexOf(Value target, Value[] parameters) {
			if(parameters.Length < 1)
				return Int(-1);
			string subString = parameters[0].str();
			return target.value != null ? Int(target.getValue<string>().IndexOf(subString)) : Int(-1);
		}

		public static Value native_length(Value target, Value[] parameters) {
			return target.value != null ? Int(target.getValue<string>().Length) : Int(0);
		}

		public enum Encoding {
			UTF8,
			UTF16,
			UTF32,
			ASCII
		}
	}
}