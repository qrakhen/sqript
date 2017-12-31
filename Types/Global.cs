using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public abstract class GlobalFunqtion : Funqtion
    {
        public GlobalFunqtion() : base(GlobalContext.getInstance()) {

        }
    }

    public class QonfigFunqtion : GlobalFunqtion
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
    }

    public static class Qonfig
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
                case "logLevel":
                    Debug.setLoggingLevel((Debug.Level) Enum.Parse(typeof(Debug.Level), value));
                    break;
            }
        }

        public static void resetValue(string key) {
            setValue(key, getDefaultValue(key));
        }
    }

    public class GlobalContext : Funqtion
    {
        private static GlobalContext instance;

        private List<Statement> queued;

        static GlobalContext() {
            getInstance().set("qonfig", new Reference(new QonfigFunqtion()));
            getInstance().set("global", new Reference(getInstance()));
        }

        public static GlobalContext getInstance() {
            if (instance == null) instance = new GlobalContext();
            return instance;
        }

        public static void resetInstance() {
            instance = new GlobalContext();
        }

        public GlobalContext() : base(null) {
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
            if (queued.Count > 0) {
            Debug.spam("main context processing " + queued.Count + " queued statements...");
                foreach (Statement statement in queued) {
                    try {
                        Value r = statement.execute(this, true);
                        if (r != null) Debug.log(r.ToString(), ConsoleColor.Green);
                        statements.Add(statement);
                    } catch (Exception e) {
                        Debug.warn("encountered exception while trying to execute statement queue:");
                        queued.Clear();
                        throw e;
                    } catch (System.Exception e) {
                        Debug.warn("encountered system exception while trying to execute statement queue:");
                        queued.Clear();
                        throw e;
                    }
                }
                queued.Clear();
            }
            Debug.spam("main context total executed statement amount: " + statements.Count);
        }

        public override string ToString() {
            string r = "GLOBAL {\n";
            return r + "}";
        }

        public override string toDebug() {
            string r = "GLOBAL {\n";
            return r + "}";
        }
    }
}
