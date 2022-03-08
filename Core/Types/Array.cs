using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript {
	/***
	 
	 *~ a <~ [ 3, '5', { n <~ 't'}]; 
	 
	 *<t1>~ t <~ [ val(t1), ... ];	
		 
	 ***/
	internal class Array : Collection<int> {

		public const string CHAR_OPEN = "[", CHAR_CLOSE = "]";


		public Array(Dictionary<int, Reference> value) : base(ValueType.Array, value) { }
		public Array() : base(ValueType.Array, new Dictionary<int, Reference>()) { }


		public virtual void Add(Reference item) {
			int free = 0;
			do {
				if (this.Get(free) == null) {
					break;
				} else {
					free++;
				}
			} while (true);
			Set(free, item);
		}

		public void Add(QValue item) {
			Add(new Reference(item));
		}

		public override void Set(int key, Reference item) {
			if (key < 0) {
				Add(item);
			} else {
				base.Set(key, item);
			}
		}

		public override string ToString() {
			string r = base.ToString();
			r = "[" + r[1..^1] + "]";
			return r;
		}
	}
}
