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

        public virtual Value execute(Value[] parameters = null) {
            Debug.spam("executing function:\n" + this.ToString());
            foreach (Statement statement in statements) {
                statement.execute(this);
            }
            return null;
        }
    }

    public class MainContext : Funqtion
    {
        private List<Statement> queued;

        public MainContext() : base(null) {
            queued = new List<Statement>();
        }

        public void queue(Statement[] statements) {
            foreach (Statement statement in statements) {
                queued.Add(statement);
            }
        }

        public void queue(Statement statement) {
            queue(new Statement[] { statement });
        }

        public void execute() {
            Debug.spam("main context processing " + queued.Count + " queued statements...");
            foreach (Statement statement in queued) {
                statement.execute(this);
                statements.Add(statement);
            }
            queued.Clear();
            Debug.spam("main context total executed statement amount: " + statements.Count);
        }
    }
}
