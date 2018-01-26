using System;
using System.Collections.Generic;

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
        public override Value execute(Value[] parameters = null, Value caller = null) {
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
            Log.debug("setting value <" + value + "> for qonfig entry '" + key + "'");
            switch (key) {
                case "log":
                case "logLevel":
                    int i = 0;
                    if (Int32.TryParse(value, out i)) {
                        Log.setLoggingLevel((Log.Level) i);
                    } else Log.setLoggingLevel((Log.Level) Enum.Parse(typeof(Log.Level), value));
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

        private List<Segment> queued;

        public GlobalContext() : base(null) {
            queued = new List<Segment>();
        }

        private void init() {
            set("qonfig", new Reference(new QonfigFunqtion()));
            set("global", new Reference(getInstance()));

            Interface[] libs = Loader.loadDirectory(AppContext.BaseDirectory + "\\lib\\");
            if (libs.Length > 0) {
                foreach (var lib in libs) {
                    lib.load();
                    set(lib.name, new Reference(lib.createInterfaceContext()));
                    Log.spam("loaded external library component '" + lib.name + "' into global context");
                }
                Log.debug("successfully loaded " + libs.Length + " external libraries.");
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

        public void queue(Segment[] statements) {
            foreach (Segment statement in statements) {
                queued.Add(statement);
            }
        }

        public void queue(Segment statement) {
            queue(new Segment[] { statement });
        }

        public void clearQueue() {
            queued.Clear();
        }

        public void execute() {
            if (queued.Count > 0) {
            Log.spam("main context processing " + queued.Count + " queued statements...");
                foreach (Segment statement in queued) {
                    Value r = statement.execute(this);
                    if (r != null) Log.debug(r.ToString(), ConsoleColor.Green);
                    segments.Add(statement);
                }
                clearQueue();
            }
            Log.spam("main context total executed statement amount: " + segments.Count);
        }

        public override string ToString() {
            string r = "{\n";
            foreach (var reference in value) {
                if (reference.Value.getTrueValue() == this) continue;
                string[] lines = reference.Value.getTrueValue().ToString().Split('\n');
                r += "    " + reference.Key + ": " + lines[0] + "\n";
                for (int i = 1; i < lines.Length; i++) r += "    " + lines[i] + ",\n";
            }
            if (r.EndsWith(",\n")) r = r.Substring(0, r.Length - 2) + "\n";
            return r + "}";
        }
    }
}
