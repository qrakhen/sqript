using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Funqtion : Context
    {
        public List<Statement> statements { get; protected set; }
        public List<string> parameters { get; protected set; }

        public Funqtion(Context parent, Dictionary<string, Reference> references, List<Statement> statements) : base(parent, ValueType.FUNQTION, references) {
            this.statements = statements;
        }

        public Funqtion(Context parent) : this(parent, new Dictionary<string, Reference>(), new List<Statement>()) {}

        public Funqtion(Context parent, List<Statement> statements) : this(parent, new Dictionary<string, Reference>(), statements) { }

        public Value execute(Value[] parameters) {
            Debug.spam("executing function:\n" + this.ToString());
            foreach (Statement statement in statements) {
                statement.execute();
            }
            return null;
        }
    }
}
