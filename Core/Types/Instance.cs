using System.Collections.Generic;

namespace Qrakhen.Sqript {

	internal class Instance : Obqect {

		public Qlass Constructor { get; protected set; }


		public Instance(Qlass constructor, Qontext parent, Dictionary<string, Reference> value) : base(parent, value) {
			this.Constructor = constructor;
		}
	}
}
