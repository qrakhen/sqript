using System;
using System.IO;

namespace Qrakhen.Sqript
{
    public static class SqriptDebug
    {
        private static Level loggingLevel = Level.LOG;

        public enum Level
        {
            MUFFLE = 0,
            CRITICAL = 1,
            WARNINGS = 2,
            LOG = 4,
            VERBOSE = 8,
            PRODUCTIVE = 1,
            TESTING = 3,
            DEVELOPMENT = 7,
            ALL = 15
        }

        public static void setLoggingLevel(Level level) {
            loggingLevel = level;
        }

        private static void line(object message, ConsoleColor color = ConsoleColor.White) {
            Console.ForegroundColor = color;
            Console.WriteLine(" : " + message.ToString());
        }

        public static void error(object message) {
            if (((int)loggingLevel & (int) Level.CRITICAL) > 0) line("ERROR " + message, ConsoleColor.Red);
        }

        public static void warn(object message) {
            if (((int)loggingLevel & (int)Level.WARNINGS) > 0) line("WARN " + message, ConsoleColor.Yellow);
        }

        public static void log(object message) {
            if (((int)loggingLevel & (int)Level.LOG) > 0) line(message);
        }

        public static void spam(object message) {
            if (((int)loggingLevel & (int)Level.VERBOSE) > 0) line(message, ConsoleColor.Gray);
        }
    }

    public class SqriptRuntime
    {
        static void Main(string[] args) {
            defineKeywords();
            defineOperators();
            SqriptDebug.setLoggingLevel(SqriptDebug.Level.DEVELOPMENT);
            string content = "";
            Context global = new Context(null);
            do {
                Console.Write(" ~> ");
                content = Console.ReadLine();
                if (content == "test") content = File.ReadAllText("TestScript.sq");
                else if (content == "exit") break;
                try {
                    var nizer = new Tokenizer(content);
                    new Interpreter(global, nizer.parse()).execute();
                } catch (Exception e) {
                    SqriptDebug.error("[" + e.GetType().ToString() + "] >> " + e.Message);
                    SqriptDebug.log(e.StackTrace);
                }
            } while (content != "exit");
        }

        static void defineKeywords() {
            Keywords.define(Keyword.DECLARE, "declare", "reference", "ref", "*~");
            Keywords.define(Keyword.DESTROY, "destroy", "dereference", "del", "~:");
            Keywords.define(Keyword.NEW, "create", "new", "spawn", "~+");
            Keywords.define(Keyword.CLASS, "class");
            Keywords.define(Keyword.FUNCTION, "function", "fn");
            Keywords.define(Keyword.OBJECT, "object", "obqect");
            Keywords.define(Keyword.RETURN, "return", "<~");
        }

        static void defineOperators() {
            Operators.define(Operator.CALCULATE_ADD, delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() + b.getValue<Decimal>()); });
            Operators.define(Operator.CALCULATE_SUBTRACT, delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() - b.getValue<Decimal>()); });
            Operators.define(Operator.CALCULATE_DIVIDE, delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() / b.getValue<Decimal>()); });
            Operators.define(Operator.CALCULATE_MULTIPLY, delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() * b.getValue<Decimal>()); });
            Operators.define(Operator.ASSIGN_VALUE, delegate (Value a, Value b, Value r) { a.setValue(b.getValue(), b.type); });
            Operators.define(Operator.ASSIGN_REFERENCE, delegate (Value a, Value b, Value r) { });
        }
    }

    public class ReferenceException : Exception
    {
        private Reference reference;

        public ReferenceException(string message, Reference reference) : base(message) {
            this.reference = reference;
        }

        public override string ToString() {
            return base.ToString() + Environment.NewLine + reference;
        }
    }

    public class OperationException : Exception
    {
        public OperationException(string message) : base(message) { }
    }

    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}