using System.Collections.Generic;

namespace Qrakhen.Sqript {
	/***
	 (qroup qroupName {)

	 (access) qlass (<ExtendedClass:)Class(>) {
		(access) Class ~(...);
		(access) (<t1:)prop1(>) prop (<~ defaultValue);
		(access) (<t2:)funq1(>) funq ~(...);
	 }

	 *~ instance <~ *:Class();
	 
	 (})
	 ***/
	internal class Qlass : Qontext {
		public Dictionary<string, Property> properties;

		/// <summary>
		/// Qlass.value is a Dictionary with all STATIC references, so a qlass without any static properties won't have anything inside its value.
		/// </summary>
		/// <param name="context"></param>
		public Qlass(
				Qontext parent,
				Dictionary<string, Property> instanceProperties = null,
				Dictionary<string, Reference> staticReferences = null) : base(parent, ValueType.Qlass, staticReferences) {
			properties = (instanceProperties ?? new Dictionary<string, Property>());
		}

		public Instance instantiate() {
			return null;
		}

		public class Property {
			public string name { get; private set; }
			public Access access { get; private set; }
			public ValueType type { get; private set; }
			public Readonly defaultValue { get; private set; }

			public Property(
					string name,
					ValueType type = ValueType.Null,
					Access access = Access.PUBLIC,
					Value defaultValue = null) {
				this.name = name;
				this.type = type;
				this.defaultValue = Readonly.fromValue(defaultValue);
				this.access = access;
			}
		}
	}
}
