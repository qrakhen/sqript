using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal abstract class GlobalFunqtion : Funqtion
    {
        public GlobalFunqtion() : base(GlobalContext.getInstance()) {

        }

        public override string ToString() {
            return "()";
        }
    }

    internal class QonfigFunqtion : GlobalFunqtion
    {
        public override Value execute(Value[] parameters = null) {
            if (parameters.Length == 1) {
                Qonfig.resetValue(parameters[0].getValue().ToString());
            } else {
                Qonfig.setValue(
                    parameters[0].getValue().ToString(), 
                    parameters[1].getValue().ToString());
            }
            return null;
        }

        public override string ToString() {
            return "(key, value)";
        }
    }

    internal static class Qonfig
    {
        public static Dictionary<string, string> defaultValues = new Dictionary<string, string>();

        static Qonfig() {
            defaultValues.Add("logLevel", "INFO");
        }

        public static string getDefaultValue(string key) {
            if (!defaultValues.ContainsKey(key)) return "undefined";
            return defaultValues[key];
        }

        public static void setValue(string key, string value) {
            Debug.log("setting value <" + value + "> for qonfig entry '" + key + "'");
            switch (key) {
                case "log":
                case "logLevel":
                    int i = 0;
                    if (Int32.TryParse(value, out i)) {
                        Debug.setLoggingLevel((Debug.Level) i);
                    } else Debug.setLoggingLevel((Debug.Level) Enum.Parse(typeof(Debug.Level), value));
                    break;
            }
        }

        public static void resetValue(string key) {
            setValue(key, getDefaultValue(key));
        }
    }

    internal class GlobalContext : Funqtion
    {
        private static GlobalContext instance;

        private List<Statement> queued;

        public GlobalContext() : base(null) {
            queued = new List<Statement>();
        }

        private void init() {
            set("qonfig", new Reference(new QonfigFunqtion()));
            set("global", new Reference(getInstance()));

            Interface[] libs = Loader.loadDirectory(AppContext.BaseDirectory + "\\lib\\");
            if (libs.Length > 0) {
                foreach (var lib in libs) {
                    lib.load();
                    set(lib.name, new Reference(lib.createInterfaceContext()));
                    Debug.spam("loaded external library component '" + lib.name + "' into global context");
                }
                Debug.log("successfully loaded " + libs.Length + " external libraries.");
            }
        }

        public static GlobalContext getInstance() {
            if (instance == null) resetInstance();
            return instance;
        }

        public static void resetInstance() {
            instance = new GlobalContext();
            instance.init();
        }

        public void queue(Statement[] statements) {
            foreach (Statement statement in statements) {
                queued.Add(statement);
            }
        }

        public void queue(Statement statement) {
            queue(new Statement[] { statement });
        }

        public void clearQueue() {
            queued.Clear();
        }

        public void execute() {
            if (queued.Count > 0) {
            Debug.spam("main context processing " + queued.Count + " queued statements...");
                foreach (Statement statement in queued) {
                    Value r = statement.execute(this, true);
                    if (r != null) Debug.log(r.ToString(), ConsoleColor.Green);
                    statements.Add(statement);
                }
                clearQueue();
            }
            Debug.spam("main context total executed statement amount: " + statements.Count);
        }

        public override string ToString() {
            string r = "{\n";
            foreach (var reference in value) {
                if (reference.Value.getReference() == this) continue;
                string[] lines = reference.Value.getReference().ToString().Split('\n');
                r += "    " + reference.Key + ": " + lines[0] + "\n";
                for (int i = 1; i < lines.Length; i++) r += "    " + lines[i] + ",\n";
            }
            if (r.EndsWith(",\n")) r = r.Substring(0, r.Length - 2) + "\n";
            return r + "}";
        }

        public override string toDebug() {
            string r = "GLOBAL {\n";
            return r + "}";
        }
    }
}
