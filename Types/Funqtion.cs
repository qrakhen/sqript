using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Funqtion : Context
    {
        public List<Statement> statements { get; protected set; }
        public List<string> parameters { get; protected set; }

        public Funqtion(
                Context parent, 
                Dictionary<string, Reference> references, 
                List<Statement> statements,
                List<string> parameters = null) : base(parent, ValueType.FUNQTION, references) {
            this.statements = statements;
            this.parameters = parameters == null ? new List<string>() : parameters;
        }

        public Funqtion(Context parent) : this(parent, new Dictionary<string, Reference>(), new List<Statement>()) {}

        public Funqtion(Context parent, List<Statement> statements) : this(parent, new Dictionary<string, Reference>(), statements) { }

        public virtual Value execute(Value[] parameters = null) {
            Debug.spam("executing function:\n" + this.ToString());
            foreach (Statement statement in statements) {
                Value r = statement.execute(this);
                if (r == null) continue;
                Debug.spam("reached return statement, returning " + r.str());
                return r;
            }
            return null;
        }

        public override string ToString() {
            string r = "(";
            foreach (string parameter in parameters) r += parameter + " ";
            r = r + "{\n";
            foreach (Statement statement in statements) {
                r += "    " + statement.ToString() + "\n";
            }
            return r + "})";
        }

        public override string toDebug() {
            string r = "(";
            foreach (string parameter in parameters) r += parameter + " ";
            r = r + "{\n";
            foreach (Statement statement in statements) {
                r += "    " + statement.ToString() + "\n";
            }
            return r + "})";
        }
    }
}
