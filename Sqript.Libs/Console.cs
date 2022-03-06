using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Qrakhen.Sqript;

namespace Qrakhen.SqriptLib {
	public class ConsoleInterface : Interface {

		private ConsoleColor _consoleColor = ConsoleColor.White;

		public ConsoleInterface() : base("console") {

		}

		public Value setColor(Dictionary<string, Value> parameters) {
			object value = parameters["color"].getValue();
			if(value is string colorString) {
				_consoleColor = (ConsoleColor) Enum.Parse(typeof(ConsoleColor), colorString, true);
			} else if(value is int colorInt) {
				_consoleColor = (ConsoleColor) colorInt;
			} else {
				throw new ArgumentException($"The parameter 'color' needs to be an {typeof(int)} or {typeof(string)}!");
			}
			return null;
		}

		public Value write(Dictionary<string, Value> parameters) {
			Console.ForegroundColor = _consoleColor;
			Console.Write(parameters["value"].getValue());
			return null;
		}

		public Value writeln(Dictionary<string, Value> parameters) {
			Console.ForegroundColor = _consoleColor;
			string text = parameters["value"].getValue().ToString();
			Console.Write(Regex.Unescape(text) + Environment.NewLine);
			return null;
		}

		public Value readKey() {
			var key = Console.ReadKey();
			return new Value((int) key.KeyChar, Sqript.ValueType.Integer);
		}

		public Value read() {
			var line = Console.ReadLine();
			return new Value(line, Sqript.ValueType.String);
		}

		public override void load() {
			define(new Call(write, new string[] { "value" }, Sqript.ValueType.Null));
			define(new Call(writeln, new string[] { "value" }, Sqript.ValueType.Null));
			define(new Call(readKey, Sqript.ValueType.Integer));
			define(new Call(read, Sqript.ValueType.String));
			define(new Call(setColor, new string[] { "color" }, Sqript.ValueType.Null));
		}
	}
}
