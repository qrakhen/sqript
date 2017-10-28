using System;
using System.IO;

namespace Qrakhen.Sqript
{
    public static class SqriptDebug
    {
        private static Level loggingLevel = Level.DEVELOPMENT;

        public enum Level
        {
            MUFFLE = 0,
            CRITICAL = 1,
            WARNINGS = 2,
            DEVELOPMENT = 4,
            VERBOSE = 8
        }

        public static void setLoggingLevel(Level level) {
            loggingLevel = level;
        }

        private static void line(string message) {
            Console.WriteLine(" : " + message);
        }

        public static void error(string message) {
            if ((int) loggingLevel >= (int) Level.CRITICAL) line("ERROR " + message);
        }

        public static void warn(string message) {
            if ((int)loggingLevel >= (int)Level.WARNINGS) line("WARN " + message);
        }

        public static void log(string message) {
            if ((int)loggingLevel >= (int)Level.DEVELOPMENT) line(message);
        }

        public static void spam(string message) {
            if ((int)loggingLevel >= (int)Level.VERBOSE) line(message);
        }
    }

    public class SqriptRuntime
    {
        static void Main(string[] args) {
            defineKeywords();
            defineOperators();
            string content = "";
            do {
                Console.Write(" ~> ");
                content = Console.ReadLine();
                if (content == "test") content = File.ReadAllText("TestScript.sq");
                else if (content == "exit") break;
                try {
                    var nizer = new Tokenizer(content);
                    var result = nizer.parse();
                    new Interpreter(null, result).execute();
                } catch (Exception e) {
                    Console.WriteLine("exception thrown >> " + e.Message);
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
            Operators.define(Operator.CALCULATE_SUBSTRACT, delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() - b.getValue<Decimal>()); });
            Operators.define(Operator.CALCULATE_DIVIDE, delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() / b.getValue<Decimal>()); });
            Operators.define(Operator.CALCULATE_MULTIPLY, delegate (Value a, Value b, Value r) { r.setValue(a.getValue<Decimal>() * b.getValue<Decimal>()); });
            Operators.define(Operator.ASSIGN, delegate (Value a, Value b, Value r) { });
            Operators.define(Operator.REFERENCE, delegate (Value a, Value b, Value r) { });
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