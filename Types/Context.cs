using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Context : Collection<string, Reference>
    {
        public Context parent { get; protected set; }

        public Context(Context parent, Dictionary<string, Reference> value) : base(ValueType.CONTEXT, value) {
            this.parent = parent;
        }

        public Context(Context parent = null) : base(ValueType.CONTEXT, new Dictionary<string, Reference>()) {
            this.parent = parent;
        }

        public override void set(string key, Reference reference) {
            if (value.ContainsKey(key)) throw new ReferenceException("can not redeclare reference", reference);
            else base.set(key, reference);
        }

        public override void remove(string key) {
            if (!value.ContainsKey(key)) throw new ReferenceException("can not destroy undeclared reference", null);
            else base.remove(key);
        }

        public Reference get(string name, bool recursive = true) {
            if (value.ContainsKey(name)) return value[name];
            else if (recursive && parent != null) return parent.get(name);
            else return null;
        }
    }
}
