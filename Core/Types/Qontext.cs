using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript {
	internal abstract class Qontext : Collection<string> {
		public readonly bool extendable;
		public Qontext parent { get; protected set; }

		public Qontext(Qontext parent, ValueType type, Dictionary<string, Reference> value, bool extendable = true) : base(type, value) {
			this.parent = parent;
			this.extendable = extendable;
		}

		public Qontext(Qontext parent, bool extendable = true) : base(ValueType.Qontext, new Dictionary<string, Reference>()) {
			this.parent = parent;
			this.extendable = extendable;
		}

		public override void set(string key, Reference reference) {
			if(value.ContainsKey(key))
				throw new ReferenceException("can not redeclare reference", reference);
			else
				base.set(key, reference);
		}

		public override void remove(string key) {
			if(!value.ContainsKey(key))
				throw new ReferenceException("can not destroy undeclared reference", null);
			else
				base.remove(key);
		}

		public Reference lookup(string name, bool recursive = true) {
			if(value.ContainsKey(name))
				return value[name];
			else if(recursive && parent != null)
				return parent.lookup(name, true);
			else
				return null;
		}

		public Reference lookupOrThrow(string name, bool recursive = true) {
			if(value.ContainsKey(name))
				return value[name];
			else if(recursive && parent != null)
				return parent.lookup(name, true);
			else
				throw new QontextException("could not lookup identifier '" + name + "' in current context");
		}

		public Reference get(string[] keys) {
			Qontext c = this;
			for(int i = 0; i < keys.Length - 1; i++) {
				if(c.value.ContainsKey(keys[i])) {
					if(c.value[keys[i]].getValue() is Qontext) {
						c = c.value[keys[i]].getValue<Qontext>();
					}
				} else
					return null;
			}
			if(c.value.ContainsKey(keys[keys.Length - 1])) {
				return c.value[keys[keys.Length - 1]];
			}
			return null;
		}

		public override Reference get(string key) {
			if(Keywords.isAlias(key, Keyword.PARENT_CONTEXT))
				return new Reference(parent);
			else if(Keywords.isAlias(key, Keyword.CURRENT_CONTEXT))
				return new Reference(this);
			else if(value.ContainsKey(key))
				return value[key];
			else if(parent != null)
				return parent.get(key);
			else
				return null;
		}
	}

	internal class StatiqQontext : Qontext {
		public StatiqQontext(Qontext parent) : base(parent) { }
	}

	public class QontextException : Exception {
		public QontextException(string message) : base(message) { }
	}
}
