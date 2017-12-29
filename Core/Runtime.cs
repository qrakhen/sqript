using System;
using System.IO;

namespace Qrakhen.Sqript
{
    public static class Sqript
    {
        public const string asciiLogo =
            "  _______/ ^ \\___.___,\n" +
            " (__.   ._ .     |     \n" +
            " .__)(_][  | [_) | _. \n" +
            "  \\    |     |     / \n";
    }

    public static class Debug
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

        public static void write(object message, ConsoleColor color = ConsoleColor.White) {
            string[] lines = message.ToString().Split(new char[] { '\n' });
            foreach (string line in lines) {
                Console.ForegroundColor = color;
                Console.WriteLine("    " + line);
            }
        }

        private static void writeOut(object message, ConsoleColor color = ConsoleColor.White) {
            string[] lines = message.ToString().Split(new char[] { '\n' });
            foreach (string line in lines) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" ~> ");
                Console.ForegroundColor = color;
                Console.WriteLine(line);
            }
        }

        public static void error(object message, ConsoleColor color = ConsoleColor.Red) {
            if (((int)loggingLevel >= (int) Level.CRITICAL)) writeOut("ERROR " + message, color);
        }

        public static void warn(object message, ConsoleColor color = ConsoleColor.Yellow) {
            if (((int)loggingLevel >= (int)Level.WARNINGS)) writeOut("WARN " + message, color);
        }

        public static void log(object message, ConsoleColor color = ConsoleColor.White) {
            if (((int)loggingLevel >= (int)Level.LOG)) writeOut(message, color);
        }

        public static void spam(object message, ConsoleColor color = ConsoleColor.Gray) {
            if (((int)loggingLevel >= (int)Level.VERBOSE)) writeOut(message, color);
        }
    }

    public class Runtime
    {
        public struct Reader
        {
            public string file;
            public Token token;
        }

        public static Reader reader = new Reader {
            file = "",
            token = null
        };

        static void Main(string[] args) {
            defineKeywords();
            defineOperators();
            Debug.setLoggingLevel(Debug.Level.VERBOSE);
            Debug.write("\n" + Sqript.asciiLogo + "");
            string content = "";
            MainContext main = new MainContext();
            do {
                try {
                    if (args.Length > 0) {
                        reader.file = args[0];
                        content = File.ReadAllText(args[0]);
                    } else {
                        reader.file = "stdin";
                        Console.Write(" <~ ");
                        content = Console.ReadLine();
                        if (content == "test") content = File.ReadAllText("TestScript.sq");
                        else if (content == "exit") break;
                    }
                    var nizer = new Tokenizer(content);
                    var stack = nizer.parse();
                    main.queue(new Interpreter(stack).parse());
                    main.execute();
                } catch (Exception e) {
                    Debug.warn("exception thrown in file " + reader.file + " at " + e.getLocation());
                    if (e.cause != null) Debug.log("caused by token " + e.cause.toDebug() + e.cause.getLocation());
                    else if (reader.token != null) Debug.log("cause unknown - last read token: " + reader.token.toDebug() + reader.token.getLocation());
                    Debug.error("[" + e.GetType().ToString() + "] " + e.Message);
                    Debug.log(e.StackTrace);
                } catch (System.Exception e) {
                    Debug.error("!SYS_EXCEPTION! [" + e.GetType().ToString() + "] " + e.Message);
                    Debug.log(e.StackTrace);
                } 
            } while (content != "exit");
        }

        static void defineKeywords() {
            Keywords.define(Keyword.DECLARE, "declare", "reference", "ref", "*~");
            Keywords.define(Keyword.DESTROY, "destroy", "dereference", "del", "~:");
            Keywords.define(Keyword.NEW, "create", "new", "spawn", "~*");
            Keywords.define(Keyword.CLASS, "class");
            Keywords.define(Keyword.FUNCTION, "function", "fn");
            Keywords.define(Keyword.OBJECT, "object", "obqect");
            Keywords.define(Keyword.RETURN, "return", "<~");
        }

        static void defineOperators() {
            Operators.define(Operator.CALCULATE_ADD, delegate (Value a, Value b, Value r) {});
            Operators.define(Operator.CALCULATE_SUBTRACT, delegate (Value a, Value b, Value r) {});
            Operators.define(Operator.CALCULATE_DIVIDE, delegate (Value a, Value b, Value r) {});
            Operators.define(Operator.CALCULATE_MULTIPLY, delegate (Value a, Value b, Value r) {});
            Operators.define(Operator.ASSIGN_VALUE, delegate (Value a, Value b, Value r) {});
            Operators.define(Operator.ASSIGN_REFERENCE, delegate (Value a, Value b, Value r) { });
        }
    }

    public class ReferenceException : Exception
    {
        private Reference reference;

        public ReferenceException(string message, Reference reference, Token cause = null) : base(message, cause) {
            this.reference = reference;
        }

        public override string ToString() {
            return base.ToString() + Environment.NewLine + reference;
        }
    }

    public class OperationException : Exception
    {
        public OperationException(string message, Token cause = null) : base(message, cause) {
        }
    }

    public class ParseException : Exception
    {
        public ParseException(string message, Token cause = null) : base(message, cause) {
        }
    }

    public class Exception : System.Exception
    {
        public Token cause;
        public Exception(string message, Token cause = null) : base(message) {
            this.cause = cause;
        }

        public string getLocation() {
            return cause == null ? "unknown" : "line " + cause.line + " at column " + cause.col;
        }
    }
}