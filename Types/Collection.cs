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

        public virtual void addChild(K key, Value item) {
            value.Add(key, item);
        }

        public virtual void setChild(K key, Value item) {
            Value child = getChild(key);
            if (child == null) addChild(key, item);
            else value[key] = item;
        }

        public virtual void removeChild(K key) {
            value.Remove(key);
        }

        public virtual Value getChild(K key) {
            return value.ContainsKey(key) ? value[key] : null;
        }
    }
}
