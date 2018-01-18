using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal class Funqtion : Context
    {
        public List<Statement> statements { get; protected set; }
        public List<string> parameters { get; protected set; }

        public Funqtion(
                Context parent, 
                Dictionary<string, Reference> references, 
                List<Statement> statements,
                List<string> parameters = null,
                ValueType type = ValueType.FUNQTION) : base(parent, type, references) {
            this.statements = statements;
            this.parameters = parameters == null ? new List<string>() : parameters;
        }

        public Funqtion(Context parent, ValueType type) : this(parent, null, null, null, type) { }

        public Funqtion(Context parent) : this(parent, new Dictionary<string, Reference>(), new List<Statement>()) {}

        public Funqtion(Context parent, List<Statement> statements) : this(parent, new Dictionary<string, Reference>(), statements) { }

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
            foreach (Statement statement in statements) {
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
                    r += parameter + ", ";
                if (r.Length > 1) r = r.Substring(0, r.Length - 2);
            }
            if (statements != null) {
                r = r + "{\n";
                foreach (Statement statement in statements) {
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

        public NativeCall(Context parent, string call) : base(parent, ValueType.NATIVE_CALL) {
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