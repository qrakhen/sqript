using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Qrakhen.Sqript
{
    internal static class SQRIPT
    {
        public const string asciiLogo =
            "  _______/ ^ \\___.___,\n" +
            " (__.   ._ .     |     \n" +
            " .__)(_][  | [_) | _.  \n" +
            "  \\    |     |     /  \n\n" +
            "     qrakhen.net       \n\n";
    }

    internal static class Log
    {
        private static Level loggingLevel = Level.INFO;

        public enum Level
        {
            MUFFLE = 0,
            CRITICAL = 1,
            WARNINGS = 2,
            INFO = 3,
            DEBUG = 4,
            VERBOSE = 5
        }

        public static void logToFile(string name, string content) {
            if (!Directory.Exists("log")) Directory.CreateDirectory("log");
            File.WriteAllText("log\\" + name + "_" + DateTime.Now.ToFileTimeUtc(), content);
        }

        public static void setLoggingLevel(Level level) {
            loggingLevel = level;
        }

        public static void write(object message, ConsoleColor color = ConsoleColor.White, string newLineSeperator = "\n", string prefix = "") {
            string[] lines = message.ToString().Split(new char[] { '\n' });
            foreach (string line in lines) {
                Console.ForegroundColor = color;
                Console.Write(prefix + line + newLineSeperator);
            }
        }

        public static void write(Level level, object message, ConsoleColor color = ConsoleColor.White, string newLineSeperator = "\n", string prefix = "") {
            if (((int)loggingLevel >= (int)level)) write(message, color, newLineSeperator, prefix);
        }

        private static void writeOut(object message, ConsoleColor color = ConsoleColor.White) {
            string[] lines = message.ToString().Split(new char[] { '\n' });
            foreach (string line in lines) {
                Console.ForegroundColor = color;
                Console.Write(" ~> ");
                write(line, color);
            }
        }

        public static void error(object message, ConsoleColor color = ConsoleColor.Red) {
            if (((int)loggingLevel >= (int) Level.CRITICAL)) writeOut("ERR " + message, color);
        }

        public static void warn(object message, ConsoleColor color = ConsoleColor.Yellow) {
            if (((int)loggingLevel >= (int)Level.WARNINGS)) writeOut("WRN " + message, color);
        }

        public static void info(object message, ConsoleColor color = ConsoleColor.White) {
            if (((int)loggingLevel >= (int)Level.INFO)) writeOut(message, color);
        }

        public static void debug(object message, ConsoleColor color = ConsoleColor.Gray) {
            if (((int)loggingLevel >= (int)Level.DEBUG)) writeOut(message, color);
        }

        public static void spam(object message, ConsoleColor color = ConsoleColor.DarkGray) {
            if (((int)loggingLevel >= (int)Level.VERBOSE)) writeOut(message, color);
        }
    }

    public static class KeyState
    {
        public enum Keys
        {
            Modifiers = -65536,
            None = 0,
            LButton = 1,
            RButton = 2,
            Cancel = 3,
            MButton = 4,
            XButton1 = 5,
            XButton2 = 6,
            Back = 8,
            Tab = 9,
            LineFeed = 10,
            Clear = 12,
            Return = 13,
            Enter = 13,
            ShiftKey = 16,
            ControlKey = 17,
            Menu = 18,
            Pause = 19,
            Capital = 20,
            CapsLock = 20,
            KanaMode = 21,
            HanguelMode = 21,
            HangulMode = 21,
            JunjaMode = 23,
            FinalMode = 24,
            HanjaMode = 25,
            KanjiMode = 25,
            Escape = 27,
            IMEConvert = 28,
            IMENonconvert = 29,
            IMEAccept = 30,
            IMEAceept = 30,
            IMEModeChange = 31,
            Space = 32,
            Prior = 33,
            PageUp = 33,
            Next = 34,
            PageDown = 34,
            End = 35,
            Home = 36,
            Left = 37,
            Up = 38,
            Right = 39,
            Down = 40,
            Select = 41,
            Print = 42,
            Execute = 43,
            Snapshot = 44,
            PrintScreen = 44,
            Insert = 45,
            Delete = 46,
            Help = 47,
            D0 = 48,
            D1 = 49,
            D2 = 50,
            D3 = 51,
            D4 = 52,
            D5 = 53,
            D6 = 54,
            D7 = 55,
            D8 = 56,
            D9 = 57,
            A = 65,
            B = 66,
            C = 67,
            D = 68,
            E = 69,
            F = 70,
            G = 71,
            H = 72,
            I = 73,
            J = 74,
            K = 75,
            L = 76,
            M = 77,
            N = 78,
            O = 79,
            P = 80,
            Q = 81,
            R = 82,
            S = 83,
            T = 84,
            U = 85,
            V = 86,
            W = 87,
            X = 88,
            Y = 89,
            Z = 90,
            LWin = 91,
            RWin = 92,
            Apps = 93,
            Sleep = 95,
            NumPad0 = 96,
            NumPad1 = 97,
            NumPad2 = 98,
            NumPad3 = 99,
            NumPad4 = 100,
            NumPad5 = 101,
            NumPad6 = 102,
            NumPad7 = 103,
            NumPad8 = 104,
            NumPad9 = 105,
            Multiply = 106,
            Add = 107,
            Separator = 108,
            Subtract = 109,
            Decimal = 110,
            Divide = 111,
            F1 = 112,
            F2 = 113,
            F3 = 114,
            F4 = 115,
            F5 = 116,
            F6 = 117,
            F7 = 118,
            F8 = 119,
            F9 = 120,
            F10 = 121,
            F11 = 122,
            F12 = 123,
            F13 = 124,
            F14 = 125,
            F15 = 126,
            F16 = 127,
            F17 = 128,
            F18 = 129,
            F19 = 130,
            F20 = 131,
            F21 = 132,
            F22 = 133,
            F23 = 134,
            F24 = 135,
            NumLock = 144,
            Scroll = 145,
            LShiftKey = 160,
            RShiftKey = 161,
            LControlKey = 162,
            RControlKey = 163,
            LMenu = 164,
            RMenu = 165,
            BrowserBack = 166,
            BrowserForward = 167,
            BrowserRefresh = 168,
            BrowserStop = 169,
            BrowserSearch = 170,
            BrowserFavorites = 171,
            BrowserHome = 172,
            VolumeMute = 173,
            VolumeDown = 174,
            VolumeUp = 175,
            MediaNextTrack = 176,
            MediaPreviousTrack = 177,
            MediaStop = 178,
            MediaPlayPause = 179,
            LaunchMail = 180,
            SelectMedia = 181,
            LaunchApplication1 = 182,
            LaunchApplication2 = 183,
            OemSemicolon = 186,
            Oem1 = 186,
            Oemplus = 187,
            Oemcomma = 188,
            OemMinus = 189,
            OemPeriod = 190,
            OemQuestion = 191,
            Oem2 = 191,
            Oemtilde = 192,
            Oem3 = 192,
            OemOpenBrackets = 219,
            Oem4 = 219,
            OemPipe = 220,
            Oem5 = 220,
            OemCloseBrackets = 221,
            Oem6 = 221,
            OemQuotes = 222,
            Oem7 = 222,
            Oem8 = 223,
            OemBackslash = 226,
            Oem102 = 226,
            ProcessKey = 229,
            Packet = 231,
            Attn = 246,
            Crsel = 247,
            Exsel = 248,
            EraseEof = 249,
            Play = 250,
            Zoom = 251,
            NoName = 252,
            Pa1 = 253,
            OemClear = 254,
            KeyCode = 65535,
            Shift = 65536,
            Control = 131072,
            Alt = 262144
        }

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static State[] keys = new State[Enum.GetValues(typeof(Keys)).Length];
        private static System.Threading.Timer timer;

        public struct State
        {
            public int keyCode;
            public string keyName;
            public byte state;
        }

        public static void run() {
            timer = new Timer(tick, null, 0, 1);
            int __idx = 0;
            foreach (int __key in Enum.GetValues(typeof(Keys))) {
                keys[__idx++] = new State {
                    keyCode = __key,
                    keyName = Enum.GetName(typeof(Keys), __key),
                    state = 0
                };
            }
        }

        public static bool keyDown(int keyCode) {
            foreach (State key in keys) {
                if (key.keyCode == keyCode) return (key.state == 1);
            }
            return false;
        }

        public static bool keyDown(string keyName) {
            foreach (State key in keys) {
                if (key.keyName == keyName) return (key.state == 1);
            }
            return false;
        }

        private static void tick(object state) {
            for (int i = 0; i < keys.Length; i++) {
                int __k = GetAsyncKeyState(keys[i].keyCode);
                if (__k == 1 || __k == Int16.MinValue) keys[i].state = 1;
                else keys[i].state = 0;
            }
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

        static void asyncInput() {

        }

        static void execute(string content, bool __DEV_DEBUG = false) {
            if (content.StartsWith("!!")) {
                content = content.Substring(2);
                __DEV_DEBUG = true;
            } else if (content.EndsWith("!!")) {
                content = content.Substring(0, content.Length - 2);
                __DEV_DEBUG = true;
            }

            if (__DEV_DEBUG) {
                var nizer = new Tokenizer(content);
                var stack = nizer.parse();
                GlobalContext.getInstance().queue(new Statementizer(stack).parse(GlobalContext.getInstance()));
                GlobalContext.getInstance().execute();
            } else {
                try {
                    var nizer = new Tokenizer(content);
                    var stack = nizer.parse();
                    GlobalContext.getInstance().queue(new Statementizer(stack).parse(GlobalContext.getInstance()));
                    GlobalContext.getInstance().execute();
                } catch (Exception e) {
                    GlobalContext.getInstance().clearQueue();
                    Log.warn("exception thrown in file " + reader.file + " at " + e.getLocation());
                    if (e.cause != null) Log.debug("(probably) caused by token " + e.cause.ToString() + e.cause.getLocation());
                    else if (reader.token != null) Log.debug("cause unknown - last read token: " + reader.token.ToString() + reader.token.getLocation());
                    Log.error("[" + e.GetType().ToString() + "] " + e.Message);
                    Log.debug(e.StackTrace);
                } catch (System.Exception e) {
                    GlobalContext.getInstance().clearQueue();
                    //Debug.error("!SYS_EXCEPTION! [" + e.GetType().ToString() + "] " + e.Message);
                    Log.error("exception thrown in file " + reader.file);
                    Log.error("this is a system exception and should not happen - writing to logs.");
                    Log.logToFile("sys_err", e.ToString());
                    Log.debug(e.StackTrace);
                }
            }
        }

        static void cli() {
            Log.setLoggingLevel(Log.Level.VERBOSE);
            Log.write("\n" + SQRIPT.asciiLogo + "", ConsoleColor.Green, "\n", "    ");
            Log.debug("  available cli commands:");
            Log.debug("   - #help");
            Log.debug("   - #run <filename>");
            Log.debug("   - #clr (clears global context)");
            Log.debug("\n  use qonfig:('log', '5'); for verbose output\n");

            string content = "";
            GlobalContext.resetInstance();
            do {
                reader.file = "stdin";
                content = "";
                Log.write(" <~ ", ConsoleColor.White, "");
                ConsoleKeyInfo c;
                do {
                    string line = Console.ReadLine();
                    if (line == "" && content == "") {
                        Console.SetCursorPosition(4, Console.CursorTop - 1);
                    } else {
                        content += line;
                        if (!KeyState.keyDown((int)KeyState.Keys.ShiftKey)) break;
                        content += "\n";
                        Log.write("    ", ConsoleColor.White, "");
                    }
                } while (c.Key != ConsoleKey.Escape);
                if (content.StartsWith("#run")) content = File.ReadAllText(content.Substring(5) + (content.EndsWith(".sq") ? "" : ".sq"));
                else if (content == "#clr") {
                    GlobalContext.resetInstance();
                    continue;
                } else if (content == "#help") {
                    Log.debug("~ HELP ~\n");
                    Log.debug("~ Keywords:");
                    foreach (var keyword in Keywords.get()) {
                        Log.debug("  > " + keyword.name + ":\n    aliases:");
                        foreach (var alias in keyword.aliases) Log.debug("    - " + alias);
                        Log.debug(" ----- ");
                    }
                    Log.debug("\n~ Tip: type 'global' to print out the global context");
                } else if (content == "#exit") break;
                execute(content);
            } while (content != "exit");
        }

        static void Main(string[] args) {
            Core.defineKeywords();
            Core.defineOperators();
            KeyState.run();
            if (args.Length == 0) cli();
            else execute(File.ReadAllText(args[0]));           
        }
    }

    internal class ConditionException : Exception
    {
        public ConditionException(string message, Token cause = null) : base(message, cause) { }
    }

    internal class ReferenceException : Exception
    {
        private Reference reference;

        public ReferenceException(string message, Reference reference, Token cause = null) : base(message, cause) {
            this.reference = reference;
        }

        public override string ToString() {
            return base.ToString() + Environment.NewLine + reference;
        }
    }

    internal class OperationException : Exception
    {
        public OperationException(string message, Token cause = null) : base(message, cause) {
        }
    }

    internal class ParseException : Exception
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