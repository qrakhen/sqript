using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public abstract class Context : Collection<string, Reference>
    {
        public Context parent { get; protected set; }

        public Context(Context parent, ValueType type, Dictionary<string, Reference> value) : base(type, value) {
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

        public Reference lookup(string name, bool recursive = true) {
            if (value.ContainsKey(name)) return value[name];
            else if (recursive && parent != null) return parent.lookup(name, true);
            else return null;
        }

        public Reference get(string[] keys) {
            Context c = this;
            for (int i = 0; i < keys.Length - 1; i++) {
                if (c.value.ContainsKey(keys[i])) {
                    if (c.value[keys[i]].getValue() is Context) {
                        c = c.value[keys[i]].getValue<Context>();
                    }
                } else return null;
            }
            if (c.value.ContainsKey(keys[keys.Length - 1])) {
                return c.value[keys[keys.Length - 1]];
            }
            return null;
        }

        public override Reference get(string key) {
            if (value.ContainsKey(key)) return value[key];
            else return null;
        }

        public override string ToString() {
            string r = "{";
            foreach (var v in value) {
                if (v.Value.getValue() == this) continue;
                r += "\n" + v.Key + " = " + v.Value.ToString() + ",";
            }
            if (r.Length > 1) r = r.Substring(0, r.Length - 1);
            return r + "\n}";
        }

        public override string toDebug() {
            string r = base.toDebug() + "{";
            foreach (var v in value) {
                if (v.Value.getValue() == this) continue;
                r += "\n" + v.Key + " = " + v.Value.toDebug() + ",";
            }
            return r.Substring(0, r.Length - 1) + "\n}";
        }
    }
}
