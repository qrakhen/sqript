using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{

	internal class Collection<K> : QValue<Dictionary<K, Reference>>
	{

		public const string MEMBER_DELIMITER = ":";

		public int Size {
			get { return (Value as Dictionary<K, Reference>).Count; }
		}


		public Collection(ValueType type, Dictionary<K, Reference> value) : base(type, value) { }


		public virtual void Set(K key, Reference item) {
			Reference _item = Get(key);
			if (_item == null)
				Value.Add(key, item);
			else {
				Value[key] = item;
			}
		}

		public virtual void Remove(K key) {
			Value.Remove(key);
		}

		public virtual Reference Get(K key) {
			return Value.ContainsKey(key) ? Value[key] : null;
		}

		public virtual Reference GetOrThrow(K key) {
			if (Value.ContainsKey(key)) {
				return Value[key];
			} else {
				throw new QontextException("unkown identifier or index '" + key + "' in given context or array");
			}
		}

		public override string ToString() {
			string r = "{\n";
			foreach (var reference in Value) {
				if (reference.Value.GetTrueValue() == this)
					continue;
				string[] lines = reference.Value.GetTrueValue().ToString().Split('\n');
				r += "	" + reference.Key + ": " + lines[0] + "\n";
				for (int i = 1; i < lines.Length; i++)
					r += "	" + lines[i] + "\n";
			}
			return r + "}";
		}
	}
}
