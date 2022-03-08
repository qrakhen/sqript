using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Qrakhen.Sqript {

	internal static class SQRIPT {

		public const string asciiLogo =
			"  _______/ ^ \\___.___,\n" +
			" (__.   ._ .	 |	    \n" +
			" .__)(_][  | [_) | _.  \n" +
			"  \\	|	 |	 /      \n\n" +
			"	 qrakhen.net	    \n\n";
	}

	internal static class Log {

		public static Level LoggingLevel { get; private set; } = Level.DEBUG;

		public enum Level {
			MUFFLE = 0,
			CRITICAL = 1,
			WARNINGS = 2,
			INFO = 3,
			DEBUG = 4,
			VERBOSE = 5
		}

		public static void LogToFile(string name, string content) {
			if (!Directory.Exists("log"))
				Directory.CreateDirectory("log");
			File.WriteAllText("log\\" + name + "_" + DateTime.Now.ToFileTimeUtc() + ".loq", content);
		}

		public static void SetLoggingLevel(Level level) {
			LoggingLevel = level;
		}

		public static void Write(object message, ConsoleColor color = ConsoleColor.White, string newLineSeperator = "\n", string prefix = "") {
			string[] lines = message.ToString().Split(new char[] { '\n' });
			foreach (string line in lines) {
				Console.ForegroundColor = color;
				Console.Write(prefix + line + newLineSeperator);
			}
		}

		public static void Write(Level level, object message, ConsoleColor color = ConsoleColor.White, string newLineSeperator = "\n", string prefix = "") {
			if (((int) LoggingLevel >= (int) level))
				Write(message, color, newLineSeperator, prefix);
		}

		private static void WriteOut(object message, ConsoleColor color = ConsoleColor.White) {
			string[] lines = message.ToString().Split(new char[] { '\n' });
			foreach (string line in lines) {
				Console.ForegroundColor = color;
				Console.Write(" ~> ");
				Write(line, color);
			}
		}

		public static void Error(object message, ConsoleColor color = ConsoleColor.Red) {
			if (((int) LoggingLevel >= (int) Level.CRITICAL))
				WriteOut("ERROR " + message, color);
		}

		public static void Warn(object message, ConsoleColor color = ConsoleColor.Yellow) {
			if (((int) LoggingLevel >= (int) Level.WARNINGS))
				WriteOut("ALERT " + message, color);
		}

		public static void Info(object message, ConsoleColor color = ConsoleColor.White) {
			if (((int) LoggingLevel >= (int) Level.INFO))
				WriteOut(message, color);
		}

		public static void Debug(object message, ConsoleColor color = ConsoleColor.Gray) {
			if (((int) LoggingLevel >= (int) Level.DEBUG))
				WriteOut(message, color);
		}

		public static void Spam(object message, ConsoleColor color = ConsoleColor.DarkGray) {
			if (((int) LoggingLevel >= (int) Level.VERBOSE))
				WriteOut(message, color);
		}
	}

	public static class KeyState {

		public enum Keys {
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

		private static readonly State[] _keys = new State[Enum.GetValues(typeof(Keys)).Length];
		private static Timer _timer;

		public struct State {
			public int KeyCode;
			public string KeyName;
			public byte StateCode;
		}


		public static void Run() {
			_timer = new Timer(Tick, null, 0, 1);
			int __idx = 0;
			foreach (int __key in Enum.GetValues(typeof(Keys))) {
				_keys[__idx++] = new State {
					KeyCode = __key,
					KeyName = Enum.GetName(typeof(Keys), __key),
					StateCode = 0
				};
			}
		}

		public static bool KeyDown(int keyCode) {
			foreach (State key in _keys) {
				if (key.KeyCode == keyCode)
					return (key.StateCode == 1);
			}
			return false;
		}

		public static bool KeyDown(string keyName) {
			foreach (State key in _keys) {
				if (key.KeyName == keyName) {
					return (key.StateCode == 1);
				}
			}
			return false;
		}

		private static void Tick(object state) {
			for (int i = 0; i < _keys.Length; i++) {
				int __k = GetAsyncKeyState(_keys[i].KeyCode);
				if (__k == 1 || __k == short.MinValue) {
					_keys[i].StateCode = 1;
				} else {
					_keys[i].StateCode = 0;
				}
			}
		}
	}

	public static class Watcher {

		private static readonly List<Diag> _log = new List<Diag>();

		public struct Diag {

			public long Mem;
			public double Cpu;
			public int Thr;

			public override string ToString() {
				string r = "";
				r += "Mem: " + Mem + " M\n";
				r += "Pct: " + Cpu + " ms\n";
				r += "Thr: " + Thr + " x";
				return r;
			}

			public string ToString(Diag compare) {
				string r = "";
				var _mem = Mem - compare.Mem;
				var _cpu = Cpu - compare.Cpu;
				var _thr = Thr - compare.Thr;
				r += "Mem: " + Mem + " M (" + (_mem < 0 ? "-" : "+") + _mem + ")\n";
				r += "Pct: " + Cpu + " ms (" + (_cpu < 0 ? "-" : "+") + _cpu + ")\n";
				r += "Thr: " + Thr + " x (" + (_thr < 0 ? "-" : "+") + _thr + ")\n";
				return r;
			}
		}


		public static string GetDiagString() {
			if (_log.Count == 1) {
				return _log[0].ToString();
			} else if (_log.Count > 1) {
				return _log[0].ToString(_log[1]);
			} else {
				return "";
			}
		}

		public static Diag Diagnose() {
			var proc = Process.GetCurrentProcess();
			Diag now = new Diag {
				Mem = proc.WorkingSet64 / 1024 / 1024,
				Cpu = proc.TotalProcessorTime.TotalMilliseconds,
				Thr = proc.Threads.Count
			};
			_log.Insert(0, now);
			return now;
		}
	}

	public class Runtime {

		public struct Reader {
			public string file;
			public Token token;
		}

		public static Reader reader = new Reader {
			file = "",
			token = null
		};

		static void AsyncInput() {

		}

		static void Execute(string content, bool __DEV_DEBUG = false) {
			if (content.StartsWith("!!")) {
				content = content[2..];
				__DEV_DEBUG = true;
			} else if (content.EndsWith("!!")) {
				content = content[0..^2];
				__DEV_DEBUG = true;
			}

			if (__DEV_DEBUG) {
				var nizer = new Tokenizer(content);
				var stack = nizer.Parse();
				GlobalContext.GetInstance().Queue(new Segmentizer(stack).Parse(GlobalContext.GetInstance()));
				GlobalContext.GetInstance().Execute();
			} else {
				try {
					var nizer = new Tokenizer(content);
					var stack = nizer.Parse();
					GlobalContext.GetInstance().Queue(new Segmentizer(stack).Parse(GlobalContext.GetInstance()));
					GlobalContext.GetInstance().Execute();
				} catch (Exception e) {
					GlobalContext.GetInstance().ClearQueue();
					Log.Warn("Exception thrown in file " + reader.file + " at " + e.GetLocation());
					if (e.cause != null)
						Log.Debug("May be caused by token " + e.cause.ToString() + e.cause.GetLocation());
					else if (reader.token != null)
						Log.Debug("Cause unknown - last read token " + reader.token.ToString() + reader.token.GetLocation());
					Log.Error("[" + e.GetType().ToString() + "] " + e.Message);
					Log.Debug(e.StackTrace);
				} catch (System.Exception e) {
					GlobalContext.GetInstance().ClearQueue();
					//Debug.error("!SYS_EXCEPTION! [" + e.GetType().ToString() + "] " + e.Message);
					Log.Error("Exception thrown in file " + reader.file);
					Log.Error("This is a system exception and should not happen - writing to logs.");
					Log.LogToFile("sys_err", e.ToString());
					Log.Debug(e.StackTrace);
				}
			}
		}

		static void Cli(string startCommand = "", bool hideHeader = false) {
			Log.SetLoggingLevel(Log.Level.DEBUG);
			if (!hideHeader) {
				Log.Write("\n" + SQRIPT.asciiLogo + "", ConsoleColor.Green, "\n", "	");
				Log.Info("  available cli commands:");
				Log.Info("   - #help");
				Log.Info("   - #run <filename>");
				Log.Info("   - #clr (clears global context)");
				Log.Info("   - #diag (process diagnose)");
				Log.Info("   - #dbg (toggle debug mode)");
				Log.Debug("\n  use qonfig('logLevel', '5'); for verbose output\n");
			}

			GlobalContext.ResetInstance();
			string content;
			do {
				reader.file = "stdin";
				content = "";
				Log.Write(" <~ ", ConsoleColor.White, "");
				// ConsoleKeyInfo c;
				do {
					string line;
					if (startCommand != "") {
						line = startCommand;
						startCommand = "";
					} else {
						line = Console.ReadLine();
					}
					if (line == "" && content == "") {
						Console.SetCursorPosition(4, Console.CursorTop - 1);
					} else {
						content += line;
						if (!KeyState.KeyDown((int) KeyState.Keys.ShiftKey))
							break;
						content += "\n";
						Log.Write("	", ConsoleColor.White, "");
					}
				} while (true);  //c.Key != ConsoleKey.Escape);
				if (content.StartsWith("#run")) {
					content = File.ReadAllText(content[5..] + (content.EndsWith(".sq") ? "" : ".sq"));
					Log.Info("Executing:");
					Log.Info(content, ConsoleColor.Cyan);
				} else if (content == "#clr") {
					GlobalContext.ResetInstance();
					continue;
				} else if (content == "#dbg") {
					Log.SetLoggingLevel((int) Log.LoggingLevel > 4 ? Log.Level.INFO : Log.Level.VERBOSE);
					continue;
				} else if (content == "#diag") {
					Watcher.Diagnose();
					Log.Info(Watcher.GetDiagString(), ConsoleColor.Cyan);
				} else if (content == "#help") {
					Log.Debug("~ HELP ~\n");
					Log.Debug("~ Keywords:");
					foreach (var keyword in Keywords.Get()) {
						Log.Debug("  > " + keyword.Name + ":\n	aliases:");
						foreach (var alias in keyword.Aliases)
							Log.Debug("	- " + alias);
						Log.Debug(" ----- ");
					}
					Log.Debug("\n~ Tip: type 'global' to print out the global context");
				} else if (content == "#exit") {
					break;
				}
				Execute(content);
				/*
				StackTrace st = new StackTrace();
				if (Log.loggingLevel == Log.Level.VERBOSE) {
					List<string> stf = new List<string>();
					for (int i = 0; i < st.FrameCount; i++) {
						var f = st.GetFrame(i);
					}
				}
				*/
			} while (content != "exit");
		}

		static void Main(string[] args) {
			Core.Init();
			KeyState.Run();
			Cli((args.Length > 0 ? "#run " + args[0] : ""), args.Length > 0);
		}
	}

	internal class ConditionException : Exception {
		public ConditionException(string message, Token cause = null) : base(message, cause) { }
	}

	internal class ReferenceException : Exception {

		private readonly Reference _reference;

		public ReferenceException(string message, Reference reference, Token cause = null) : base(message, cause) {
			this._reference = reference;
		}

		public override string ToString() {
			return base.ToString() + Environment.NewLine + _reference;
		}
	}

	internal class OperationException : Exception {
		public OperationException(string message, Token cause = null) : base(message, cause) { }
	}

	internal class ParseException : Exception {
		public ParseException(string message, Token cause = null) : base(message, cause) { }
	}

	public class Exception : System.Exception {

		public Token cause;

		public Exception(string message, Token cause = null) : base(message) {
			this.cause = cause;
		}

		public string GetLocation() {
			return cause == null ? "unknown" : "line " + (cause.line + 1) + " at column " + cause.col;
		}
	}
}