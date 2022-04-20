using System;
using System.Collections.Generic;

namespace Qrakhen.Sqript
{
	/***
	 
	 *~ f <~ ~(a b c {
		*~ d <~ a * b;
		<~ c - d;
	 });

	 *~ of <~ ~(a {
		<~ a;
	 }, a b {
		<~ a + b;
	 }, ...);
	 
	 *<t1>~ tf <~ ~(<t2:a> <t3:b> {
		<~ ...
	 });
		 
	 ***/
	internal class Funqtion : Qontext
	{

		public List<Segment> Segments { get; protected set; }
		public List<string> Parameters { get; protected set; }
		protected Func<QValue, QValue[], QValue> nativeCallback;
		public bool Returning = false;


		public Funqtion(
				Qontext parent,
				Dictionary<string, Reference> references,
				List<Segment> segments,
				List<string> parameters = null,
				ValueType type = ValueType.Funqtion) : base(parent, type, references) {
			this.Segments = segments;
			this.Parameters = parameters ?? new List<string>();
		}

		public Funqtion(Qontext parent) : this(parent, new Dictionary<string, Reference>(), new List<Segment>()) { }

		public Funqtion(Qontext parent, ValueType type) : this(parent, null, null, null, type) { }
		
		public Funqtion(Qontext parent, List<Segment> statements) : this(parent, new Dictionary<string, Reference>(), statements) { }

		public Funqtion(Qontext parent, Func<QValue, QValue[], QValue> nativeCallback) : this(parent, null, null, null, ValueType.NativeCall) {
			this.nativeCallback = nativeCallback;
		}


		public virtual QValue Execute(QValue[] parameters = null, QValue caller = null) {
			if (nativeCallback != null)
				return nativeCallback(caller, parameters);
			// we need to store all references in a temporary xfq (execution funqtion) so that the original funqtion is not mutated
			Log.Spam("executing function:\n" + this.ToString());
			Funqtion xfq = new Funqtion(Parent);
			if (parameters != null) {
				for (int i = 0; i < parameters.Length; i++) {
					if (i >= this.Parameters.Count)
						throw new Exception("more parameters provided than funqtion accepts");
					Log.Spam(this.Parameters[i] + " = " + parameters[i].Str());
					xfq.Set(this.Parameters[i], new Reference(parameters[i]));
				}
			}
			foreach (Segment s in Segments) {
				QValue r = s.Execute(xfq);
				if (s.Returning && r != null) {
					Log.Spam("reached return statement, returning " + r.Str());
					this.Returning = true;
					return r;
				}
			}
			return null;
		}

		public override string ToString() {
			string r = "(";
			if (Parameters != null) {
				foreach (string parameter in Parameters)
					r += parameter + " ";
				//if (r.Length > 1) r = r.Substring(0, r.Length - 2);
			}
			if (Segments != null) {
				r +=  "{\n";
				foreach (Segment statement in Segments) {
					r += "	" + statement.ToString() + "\n";
				}
				r += "}";
			}
			return r + ")";
		}
	}
}