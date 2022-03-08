using System.Collections.Generic;

namespace Qrakhen.Sqript {

	internal class Qroup : Qontext {

		public Dictionary<string, Property> Qlasses = new Dictionary<string, Property>();

		/// <summary>
		/// Qlass.value is a Dictionary with all STATIC references, so a qlass without any static properties won't have anything inside its value.
		/// </summary>
		/// <param name="context"></param>
		public Qroup(
				Qontext parent,
				Dictionary<string, Property> qlasses = null,
				Dictionary<string, Reference> subQroups = null
			) : base(parent, ValueType.Qlass, subQroups) {
			this.Qlasses = qlasses ?? new Dictionary<string, Property>();
		}


		public Instance Instantiate() {
			return null;
		}


		public class Property {

			public string Name { get; private set; }
			public Access Access { get; private set; }
			public Qlass Qlass { get; private set; }


			public Property(
					string name,
					Qlass qlass,
					Access access = Access.PUBLIC
				) {
				this.Name = name;
				this.Qlass = qlass;
				this.Access = access;
			}
		}
	}
}
