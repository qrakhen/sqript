using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal class Collection<K, T> : Value<Dictionary<K, T>>
    {
        public const string MEMBER_DELIMITER = ":";

        public int size {
            get { return (value as Dictionary<K, T>).Count; }
        }

        public Collection(ValueType type, Dictionary<K, T> value) : base(type, value) {
            
        }

        public virtual void set(K key, T item) {
            T _item = get(key);
            if (_item == null) value.Add(key, item);
            else value[key] = item;
        }

        public virtual void remove(K key) {
            value.Remove(key);
        }

        public virtual T get(K key) {
            return value.ContainsKey(key) ? value[key] : default(T);
        }

        public virtual T getOrThrow(K key) {
            if (value.ContainsKey(key)) return value[key];
            else throw new QontextException("unkown identifier or index '" + key + "' in given context or array");
        }

        /*public virtual Value get(object[] keys) {
            if (keys.Length < 1) throw new Exception("trying to access collection member with empty set of keys");
            Value v = null;
            Value c = this;
            foreach (object key in keys) {
                if (c is Obqect) {
                    v = (c as Obqect).get((string)key);
                } else if (c is Array) {
                    v = (c as Array).get((int)key);
                } else throw new Exception("accessing member of non-collection value");
                c = v;
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

        public override string ToString() {
            return "[" + type.ToString() + "] (" + size + ")";
        }
    }
}
