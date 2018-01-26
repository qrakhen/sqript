using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    /***
     
     *~ a <~ [ 3, '5', { n <~ 't'}]; 
     
     *<t1>~ t <~ [ val(t1), ... ];    
         
     ***/
    internal class Array : Collection<int, Reference>
    {
        public const string
            CHAR_OPEN = "[",
            CHAR_CLOSE = "]"; 

        public Array(Dictionary<int, Reference> value) : base(ValueType.Array, value) { }
        public Array() : base(ValueType.Array, new Dictionary<int, Reference>()) { }

        public virtual void add(Value item) {
            int free = 0;
            do {

            } while (get(free++) != null);
            set(free, new Reference(item));
        }

        public virtual void add(Reference item) {
            int free = 0;
            do {

            } while (get(free++) != null);
            set(free, item);
        }

        public void add(Value item) {
            add(new Reference(item));
        }

        /*public Reference query(string query, bool safe = true, bool autoCreate = true) {
            string[] keys = query.Split(MEMBER_DELIMITER.ToCharArray());
            Reference r = get(Int32.Parse(keys[0]));
            if (r == null) {
                if (autoCreate) {
                    r = new Reference();
                    set(Int32.Parse(keys[0]), r);
                } else if (safe) throw new Exception("tried to access undefined name '" + keys[0] + "'");
                else return null;
            }
            for (int i = 1; i < keys.Length; i++) {
                string key = keys[i];
                Value v = r.getTrueValue();
                if (v == null) throw new Exception("tried to access member of empty reference '" + keys[i - 1] + ":" + keys[i] + "'");
                if (v is Array) {
                    r = (v as Array).get(Int32.Parse(key));
                    if (r == null) {
                        if (autoCreate) {
                            r = new Reference();
                            (v as Array).set(Int32.Parse(key), r);
                        } else if (safe) throw new Exception("tried to access undefined array index '" + keys[i - 1] + ":" + keys[i] + "'");
                        else return null;
                    }
                } else if (v is Qontext) {
                    r = (v as Qontext).get(key);
                    if (r == null) {
                        if (autoCreate) {
                            r = new Reference();
                            (v as Qontext).set(key, r);
                        } else if (safe) throw new Exception("tried to access undefined context member '" + keys[i - 1] + ":" + keys[i] + "'");
                        else return null;
                    }
                } else throw new Exception("trying to access member of memberless value");
            }
            return r;
        }*/

        public override string ToString() {
            string r = " [ ";
            foreach (var v in value) {
                r += "\n" + v.Key + " = " + v.Value.ToString() + ",";
            }
            return r.Substring(0, r.Length - 1) + "\n]";
        }
    }
}
