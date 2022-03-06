using System;
using System.Collections.Generic;

namespace Qrakhen.Sqript {
	internal class Native {
		public static readonly Dictionary<ValueType, Qontext> native = new Dictionary<ValueType, Qontext>();

		public static void define(ValueType type, string name, Func<Value, Value[], Value> callback) {
			Funqtion fq = new Funqtion(null, callback);
			if(!native.ContainsKey(type))
				native.Add(type, new StatiqQontext(null));
			native[type].set(name, new Reference(fq));
		}

		public static Reference get(ValueType type, string name) {
			if(native.ContainsKey(type))
				return native[type].get(name);
			else if(native.ContainsKey(ValueType.Any))
				return native[ValueType.Any].get(name);
			return null;
		}
	}
}
