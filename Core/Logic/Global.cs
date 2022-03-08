using System;
using System.Collections.Generic;

namespace Qrakhen.Sqript {

	internal abstract class GlobalFunqtion : Funqtion {

		public GlobalFunqtion() : base(GlobalContext.GetInstance()) {

		}

		public override string ToString() {
			return "()";
		}
	}

	internal class QonfigFunqtion : GlobalFunqtion {
		public override QValue Execute(QValue[] parameters = null, QValue caller = null) {
			if (parameters.Length == 1) {
				Qonfig.ResetValue(parameters[0].GetValue().ToString());
			} else {
				Qonfig.SetValue(
					parameters[0].GetValue().ToString(),
					parameters[1].GetValue().ToString());
			}
			return null;
		}

		public override string ToString() {
			return "(key, value)";
		}
	}

	internal static class Qonfig {
		public static Dictionary<string, string> defaultValues = new Dictionary<string, string>();

		static Qonfig() {
			defaultValues.Add("logLevel", "INFO");
			// ToDo: Add auto #clr
		}

		public static string GetDefaultValue(string key) {
			return !defaultValues.ContainsKey(key) ? "undefined" : defaultValues[key];
		}

		public static void SetValue(string key, string value) {
			Log.Debug("setting value <" + value + "> for qonfig entry '" + key + "'");
			switch (key) {
				case "log":
				case "logLevel":
					int i;
					if (Int32.TryParse(value, out i)) {
						Log.SetLoggingLevel((Log.Level) i);
					} else {
						Log.SetLoggingLevel((Log.Level) Enum.Parse(typeof(Log.Level), value));
					}
					break;
			}
		}

		public static void ResetValue(string key) {
			SetValue(key, GetDefaultValue(key));
		}
	}

	internal class GlobalContext : Funqtion {

		private static GlobalContext _instance;

		private readonly List<Segment> _queued = new List<Segment>();

		public GlobalContext() : base(null) { }

		private void Init() {
			Set("qonfig", new Reference(new QonfigFunqtion()));
			Set("global", new Reference(GetInstance()));

			Interface[] libs = Loader.LoadDirectory(AppContext.BaseDirectory + "\\lib\\");
			if (libs.Length > 0) {
				foreach (var lib in libs) {
					lib.Load();
					Set(lib.Name, new Reference(lib.CreateInterfaceContext()));
					Log.Spam("loaded external library component '" + lib.Name + "' into global context");
				}
				Log.Debug("successfully loaded " + libs.Length + " external libraries.");
			}
		}

		public static GlobalContext GetInstance() {
			if (_instance == null)
				ResetInstance();
			return _instance;
		}

		public static void ResetInstance() {
			_instance = new GlobalContext();
			_instance.Init();
		}

		public void Queue(Segment[] statements) {
			foreach (Segment statement in statements) {
				_queued.Add(statement);
			}
		}

		public void Queue(Segment statement) {
			Queue(new Segment[] { statement });
		}

		public void ClearQueue() {
			_queued.Clear();
		}

		public void Execute() {
			if (_queued.Count > 0) {
				Log.Spam("main context processing " + _queued.Count + " queued statements...");
				foreach (Segment s in _queued) {
					QValue r = s.Execute(this);
					if (r != null && !r.IsNull())
						Log.Debug(r.ToString(), ConsoleColor.Green);
					Segments.Add(s);
				}
				ClearQueue();
			}
			Log.Spam("main context total executed statement amount: " + Segments.Count);
		}
	}
}
