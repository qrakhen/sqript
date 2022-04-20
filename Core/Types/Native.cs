using System;
using System.Collections.Generic;

namespace Qrakhen.Sqript
{

	internal class Native
	{

		public static readonly Dictionary<ValueType, Qontext> native = new Dictionary<ValueType, Qontext>();


		public static void Define(ValueType type, string name, Func<QValue, QValue[], QValue> callback) {
			Funqtion fq = new Funqtion(null, callback);
			if (!native.ContainsKey(type))
				native.Add(type, new StatiqQontext(null));
			native[type].Set(name, new Reference(fq));
		}

		public static Reference Get(ValueType type, string name) {
			if (native.ContainsKey(type))
				return native[type].Get(name);
			else if (native.ContainsKey(ValueType.Any))
				return native[ValueType.Any].Get(name);
			return null;
		}
	}
}
