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

		public Dictionary<string, Property> Properties;

		/// <summary>
		/// Qlass.value is a Dictionary with all STATIC references, so a qlass without any static properties won't have anything inside its value.
		/// </summary>
		/// <param name="context"></param>
		public Qlass(
				Qontext parent,
				Dictionary<string, Property> instanceProperties = null,
				Dictionary<string, Reference> staticReferences = null) : base(parent, ValueType.Qlass, staticReferences) {
			Properties = instanceProperties ?? new Dictionary<string, Property>();
		}


		public Instance Instantiate() {
			return null;
		}

		public class Property {

			public string Name { get; private set; }
			public Access Access { get; private set; }
			public ValueType Type { get; private set; }
			public Readonly DefaultValue { get; private set; }

			public Property(
					string name,
					ValueType type = ValueType.Null,
					Access access = Access.PUBLIC,
					QValue defaultValue = null) {
				this.Name = name;
				this.Type = type;
				this.DefaultValue = Readonly.FromValue(defaultValue);
				this.Access = access;
			}
		}
	}
}
