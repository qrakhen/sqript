using System.Collections.Generic;

namespace Qrakhen.Sqript
{

	internal class Obqect : Qontext
	{

		public Obqect(Qontext parent, Dictionary<string, Reference> value, bool extendable = true)
			: base(parent, ValueType.Obqect, value, extendable) { }

		public Obqect(Qontext parent, bool extendable = true)
			: this(parent, new Dictionary<string, Reference>(), extendable) { }

	}
}
