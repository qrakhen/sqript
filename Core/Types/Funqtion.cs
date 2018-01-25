using System.Collections.Generic;

namespace Qrakhen.Sqript
{
    internal class Funqtion : Qontext
    {
        public List<Segment> segments { get; protected set; }
        public List<string> parameters { get; protected set; }

        public Funqtion(
                Qontext parent, 
                Dictionary<string, Reference> references, 
                List<Segment> segments,
                List<string> parameters = null,
                ValueType type = ValueType.FUNQTION) : base(parent, type, references) {
            this.segments = segments;
            this.parameters = parameters ?? new List<string>();
        }

        public Funqtion(Qontext parent, ValueType type) : this(parent, null, null, null, type) { }

        public Funqtion(Qontext parent) : this(parent, new Dictionary<string, Reference>(), new List<Segment>()) {}

        public Funqtion(Qontext parent, List<Segment> statements) : this(parent, new Dictionary<string, Reference>(), statements) { }

        public virtual Value execute(Value[] parameters = null) {
            // we need to store all references in a temporary xfq (execution funqtion) so that the original funqtion is not mutated
            Log.spam("executing function:\n" + this.ToString());
            Funqtion xfq = new Funqtion(parent);
            if (parameters != null) {
                for (int i = 0; i < parameters.Length; i++) {
                    if (i >= this.parameters.Count) throw new Exception("more parameters provided than funqtion accepts");
                    Log.spam(this.parameters[i] + " = " + parameters[i].str());
                    xfq.set(this.parameters[i], new Reference(parameters[i]));
                }
            }
            foreach (Segment statement in segments) {
                Value r = statement.execute(xfq);
                if (r == null) continue;
                Log.spam("reached return statement, returning " + r.str());
                return r;
            }
            return null;
        }

        public override string ToString() {
            string r = "(";
            if (parameters != null) {
                foreach (string parameter in parameters)
                    r += parameter + " ";
                //if (r.Length > 1) r = r.Substring(0, r.Length - 2);
            }
            if (segments != null) {
                r = r + "{\n";
                foreach (Segment statement in segments) {
                    r += "    " + statement.ToString() + "\n";
                }
                r += "}";
            }
            return r + ")";
        }
    }

    internal class NativeCall : Funqtion
    {
        public string call { get; protected set; }

        public NativeCall(Qontext parent, string call) : base(parent, ValueType.NATIVE_CALL) {
            this.call = call;
        }

        public override Value execute(Value[] parameters = null) {
            return nativeCalls[call](parameters, parent);
        }

        public override string ToString() {
            return "[NATIVE] " + base.ToString();
        }
    }
}