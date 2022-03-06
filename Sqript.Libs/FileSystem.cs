using System;
using Qrakhen.Sqript;
using System.Collections.Generic;
using System.IO;

namespace Qrakhen.SqriptLib {
	public class FileInterface : Interface {
		public FileInterface() : base("file") {

		}

		public Value exists(Dictionary<string, Value> parameters) {
			return new Value(File.Exists(parameters["file"].str()), Qrakhen.Sqript.ValueType.Boolean);
		}

		public Value read(Dictionary<string, Value> parameters) {
			if(!File.Exists(parameters["file"].str()))
				throw new Qrakhen.Sqript.Exception("could not find file '" + parameters["file"] + "'");
			else
				return new Value(File.ReadAllText(parameters["file"].str()), Qrakhen.Sqript.ValueType.String);
		}

		public Value write(Dictionary<string, Value> parameters) {
			string content;
			if(parameters["content"].getValue() == null)
				content = "";
			else
				content = parameters["content"].str();
			File.WriteAllText(parameters["file"].str(), content);
			return Value.True;
		}

		public override void load() {
			define(new Call(read, new string[] { "file" }, Sqript.ValueType.String, "read"));
			define(new Call(write, new string[] { "file", "content" }, Sqript.ValueType.Boolean, "write"));
			define(new Call(exists, new string[] { "file" }, Sqript.ValueType.Boolean, "exists"));
		}
	}
}
