using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public abstract class Context : Collection<string, Reference>
    {
        public const string
            CHAR_OPEN = "{",
            CHAR_CLOSE = "}";

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

        public Reference lookupOrThrow(string name, bool recursive = true) {
            if (value.ContainsKey(name)) return value[name];
            else if (recursive && parent != null) return parent.lookup(name, true);
            else throw new ContextException("could not lookup identifier '" + name + "' in current context");
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
            if (key.Contains(MEMBER_DELIMITER)) return query(key);
            else if (value.ContainsKey(key)) return value[key];
            else return null;
        }

        public Reference query(string query, bool safe = true, bool autoCreate = true) {
            string[] keys = query.Split(MEMBER_DELIMITER.ToCharArray());
            Reference r = get(keys[0]);
            if (r == null) {
                if (autoCreate) {
                    r = new Reference();
                    set(keys[0], r);
                } else if (safe) throw new Exception("tried to access undefined name '" + keys[0] + "'");
                else return null;
            }
            for (int i = 1; i < keys.Length; i++) {
                string key = keys[i];
                Value v = r.getReference();
                if (v == null) throw new Exception("tried to access member of empty reference '" + keys[i - 1] + ":" + keys[i] + "'");
                if (v.isType(ValueType.ARRAY)) {
                    r = (v as Array).get(Int32.Parse(key));
                    if (r == null) {
                        if (autoCreate) {
                            r = new Reference();
                            (v as Array).set(Int32.Parse(key), r);
                        } else if (safe) throw new Exception("tried to access undefined array index '" + keys[i - 1] + ":" + keys[i] + "'");
                        else return null;
                    }
                } else if (v.isType(ValueType.CONTEXT)) {
                    Context ctx = (Context)v;
                    string asd = ctx.str();
                    r = (v as Context).get(key);
                    if (r == null) {
                        if (autoCreate) {
                            r = new Reference();
                            (v as Context).set(key, r);
                        } else if (safe) throw new Exception("tried to access undefined context member '" + keys[i - 1] + ":" + keys[i] + "'");
                        else return null;
                    }
                } else throw new Exception("trying to access member of memberless value type " + v.type.ToString());
            }
            return r;  
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

    public class ContextException : Exception
    {
        public ContextException(string message) : base(message) { }
    }
}
