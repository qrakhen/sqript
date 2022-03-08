using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript {

	internal abstract class Qontext : Collection<string> {

		public readonly bool extendable;
		public Qontext Parent { get; protected set; }

		public Qontext(Qontext parent, ValueType type, Dictionary<string, Reference> value, bool extendable = true) : base(type, value) {
			this.Parent = parent;
			this.extendable = extendable;
		}

		public Qontext(Qontext parent, bool extendable = true) : base(ValueType.Qontext, new Dictionary<string, Reference>()) {
			this.Parent = parent;
			this.extendable = extendable;
		}

		public override void Set(string key, Reference reference) {
			if (Value.ContainsKey(key)) {
				throw new ReferenceException("can not redeclare reference", reference);
			} else {
				base.Set(key, reference);
			}
		}

		public override void Remove(string key) {
			if (!Value.ContainsKey(key)) {
				throw new ReferenceException("can not destroy undeclared reference", null);
			} else {
				base.Remove(key);
			}
		}

		public Reference Lookup(string name, bool recursive = true) {
			if (this.Value.ContainsKey(name)) {
				return Value[name];
			} else if (recursive && Parent != null) {
				return Parent.Lookup(name, true);
			} else {
				return null;
			}
		}

		public Reference LookupOrThrow(string name, bool recursive = true) {
			if (Value.ContainsKey(name)) {
				return Value[name];
			} else if (recursive && Parent != null) {
				return Parent.Lookup(name, true);
			} else {
				throw new QontextException("could not lookup identifier '" + name + "' in current context");
			}
		}

		public Reference Get(string[] keys) {
			Qontext c = this;
			for (int i = 0; i < keys.Length - 1; i++) {
				if (c.Value.ContainsKey(keys[i])) {
					if (c.Value[keys[i]].GetValue() is Qontext) {
						c = c.Value[keys[i]].GetValue<Qontext>();
					}
				} else {
					return null;
				}
			}
			if (c.Value.ContainsKey(keys[^1])) {
				return c.Value[keys[^1]];
			}
			return null;
		}

		public override Reference Get(string key) {
			if (Keywords.IsAlias(key, Keyword.PARENT_CONTEXT)) {
				return new Reference(Parent);
			} else if (Keywords.IsAlias(key, Keyword.CURRENT_CONTEXT)) {
				return new Reference(this);
			} else if (Value.ContainsKey(key)) {
				return Value[key];
			} else if (Parent != null) {
				return Parent.Get(key);
			} else {
				return null;
			}
		}
	}

	internal class StatiqQontext : Qontext {
		public StatiqQontext(Qontext parent) : base(parent) { }
	}

	public class QontextException : Exception {
		public QontextException(string message) : base(message) { }
	}
}
