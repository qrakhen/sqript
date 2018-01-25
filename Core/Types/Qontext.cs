using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal abstract class Qontext : Collection<string, Reference>
    {
        static Qontext() {
            /*nativeCalls.Add(
                "toString", 
                new Func<Value[], Context, Value>(delegate (Value[] parameters, Context caller) {
                    return new Value(caller.ToString(), ValueType.STRING);
                })
            );
            nativeCalls.Add(
                "getType",
                new Func<Value[], Context, Value>(delegate (Value[] parameters, Context caller) {
                    return new Value(caller.type.ToString(), ValueType.STRING);
                })
            );
            nativeCalls.Add(
                "equals",
                new Func<Value[], Context, Value>(delegate (Value[] parameters, Context caller) {
                    if (parameters.Length < 1) return FALSE;
                    return new Value(caller.getValue().Equals(parameters[0].getValue()), ValueType.BOOLEAN);
                })
            );*/
        }

        public Qontext parent { get; protected set; }

        public Qontext(Qontext parent, ValueType type, Dictionary<string, Reference> value) : base(type, value) {
            this.parent = parent;
        }

        public Qontext(Qontext parent) : base(ValueType.CONTEXT, new Dictionary<string, Reference>()) {
            this.parent = parent;
        }

        protected override void assignNativeCalls() {
            foreach (var call in nativeCalls) {
                set(call.Key, new Reference(new NativeCall(this, call.Key)));
            }
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
            else throw new QontextException("could not lookup identifier '" + name + "' in current context");
        }

        public Reference get(string[] keys) {
            Qontext c = this;
            for (int i = 0; i < keys.Length - 1; i++) {
                if (c.value.ContainsKey(keys[i])) {
                    if (c.value[keys[i]].getValue() is Qontext) {
                        c = c.value[keys[i]].getValue<Qontext>();
                    }
                } else return null;
            }
            if (c.value.ContainsKey(keys[keys.Length - 1])) {
                return c.value[keys[keys.Length - 1]];
            }
            return null;
        }

        public override Reference get(string key) {
            if (Keywords.isAlias(key, Keyword.PARENT_CONTEXT)) return new Reference(parent);
            else if (Keywords.isAlias(key, Keyword.CURRENT_CONTEXT)) return new Reference(this);
            else if (value.ContainsKey(key)) return value[key];
            else return null;
        }

        public Reference query(string query, bool safe = true, bool autoCreate = true) {
            Log.error("I SHOULD STOP USING CONTEXT QUERY, IS THERE REALLY ANY SPOT WHERE I'D REALLY NEED THIS? USE RESREFREC");
            Log.spam("called context::query with parameters: '" + query + "', '" + safe + "', '" + autoCreate + "'.");
            string[] keys = query.Split(MEMBER_DELIMITER.ToCharArray());

            int index = 0;
            Reference r = get(keys[index++]);

            /*r = get(keys[index]);
            if (r == null) {
                if (index > 0) {
                    if (safe) throw new Exception("tried to access undefined member '" + keys[index] + "'");
                    else return null;
                } else {
                    r = lookup(keys[index]);
                    if (r == null)
                        if (safe) throw new Exception("tried to access undefined or inaccessible identifier '" + keys[index] + "'");
                        else if (!safe) return null;
                }
            }*/

            for (int i = index; i < keys.Length; i++) {
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
                    Qontext ctx = (Qontext)v;
                    string asd = ctx.str();
                    r = (v as Qontext).get(key);
                    if (r == null) {
                        if (autoCreate) {
                            r = new Reference();
                            (v as Qontext).set(key, r);
                        } else if (safe) throw new Exception("tried to access undefined context member '" + keys[i - 1] + ":" + keys[i] + "'");
                        else return null;
                    }
                } else throw new Exception("trying to access member of memberless value type " + v.type.ToString());
            }
            return r;  
        }

        public override string ToString() {
            string r = "{\n";
            foreach (var reference in value) {
                if (reference.Value.getReference() == (Qontext) this) continue;
                string[] lines = reference.Value.getReference().ToString().Split('\n');
                r += "    " + reference.Key + ": " + lines[0] + "\n";
                for (int i = 1; i < lines.Length; i++) r += "    " + lines[i] + "\n";
            }
            return r + "}";
        }
    }

    public class QontextException : Exception
    {
        public QontextException(string message) : base(message) { }
    }
}
