using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Collection<K> : Value<Dictionary<K, Value>>
    {
        public int size {
            get { return value.Count; }
        }

        public Collection(ValueType type, Dictionary<K, Value> value) : base(type, value) {
            
        }

        public virtual void set(K key, Value item) {
            Value child = get(key);
            if (child == null) value.Add(key, item);
            else value[key] = item;
        }

        public virtual void remove(K key) {
            value.Remove(key);
        }

        public virtual Value get(K key) {
            return value.ContainsKey(key) ? value[key] : null;
        }
        
        /*public virtual Value get(object[] keys) {
            if (keys.Length < 1) throw new Exception("trying to access collection member with empty set of keys");
            Value v = get(keys[0]);
            if (keys.Length > 1)
                foreach (K key in keys) {
                    if (v.getValueSystemType() is Collection<K>) {
                        v = (v as Collection<K>).get(key).getValue<Value>();
                    } else throw new Exception("accessing member of non-collection value");
                }
            return v;
        }*/

        /*public struct MemberSelect
        {
            public Collection<K> collection;
            public object[] select;

            public MemberSelect(Collection<K> collection, object[] select) {
                this.collection = collection;
                this.select = select;
            }

            public Value getMember() {
                K[] __keys = new K[select.Length];
                for (int i = 0; i < select.Length; i++) __keys[i] = (K)select[i];
                return collection.get(__keys);
            }
        }*/
    }
}
