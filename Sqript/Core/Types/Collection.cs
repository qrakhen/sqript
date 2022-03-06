using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript {
	internal class Collection<K> : Value<Dictionary<K, Reference>> {
		public const string MEMBER_DELIMITER = ":";

		public int size {
			get { return (value as Dictionary<K, Reference>).Count; }
		}

		public Collection(ValueType type, Dictionary<K, Reference> value) : base(type, value) {

		}

		public virtual void set(K key, Reference item) {
			Reference _item = get(key);
			if(_item == null)
				value.Add(key, item);
			else
				value[key] = item;
		}

		public virtual void remove(K key) {
			value.Remove(key);
		}

		public virtual Reference get(K key) {
			return value.ContainsKey(key) ? value[key] : null;
		}

		public virtual Reference getOrThrow(K key) {
			if(value.ContainsKey(key))
				return value[key];
			else
				throw new QontextException("unkown identifier or index '" + key + "' in given context or array");
		}

		public override string ToString() {
			string r = "{\n";
			foreach(var reference in value) {
				if(reference.Value.getTrueValue() == this)
					continue;
				string[] lines = reference.Value.getTrueValue().ToString().Split('\n');
				r += "	" + reference.Key + ": " + lines[0] + "\n";
				for(int i = 1; i < lines.Length; i++)
					r += "	" + lines[i] + "\n";
			}
			return r + "}";
		}
	}
}
