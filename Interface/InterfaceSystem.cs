using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Qrakhen.Sqript
{
    public abstract class Interface
    {
        public string name { get; private set; }
        public Dictionary<string, Call> calls;

        public Interface(string name) {
            this.name = name;
            calls = new Dictionary<string, Call>();
        }

        public abstract void load();

        public void define(Call call) {
            calls.Add(call.name, call);
        }

        public Value call(string name, Value[] parameters) {
            if (calls.ContainsKey(name)) return calls[name].execute(parameters);
            else throw new InterfaceException("could not find interface call '" + name + "'", this);
        }

        public class Call
        {
            public Func<Dictionary<string, Value>, Value> callback { get; protected set; }
            public string name { get; protected set; }
            public string[] parameters { get; protected set; }

            public Call(string name, string[] parameters, Func<Dictionary<string, Value>, Value> callback) {
                this.name = name;
                this.parameters = parameters;
                this.callback = callback;
            }

            public Value execute(Value[] parameters) {
                Dictionary<string, Value> provided = new Dictionary<string, Value>();
                for (int i = 0; i < this.parameters.Length; i++) {
                    if (parameters.Length <= i) provided.Add(this.parameters[i], null);
                    else provided.Add(this.parameters[i], parameters[i]);
                }
                return callback(provided);
            }
        }

        public class Funqtion : Sqript.Funqtion
        {
            public Call call { get; private set; }

            public Funqtion(Context parent, Call call) : base(parent) {
                this.call = call;
            }

            public override Value execute(Value[] parameters) {
                return call.execute(parameters);
            }
        }

        public Obqect createInterfaceContext() {
            Obqect context = new Obqect(null);
            foreach (var call in calls) {
                context.set(call.Key, new Reference(new Funqtion(context, call.Value)));
            }
            return context;
        }
    }

    public class InterfaceException : Exception
    {
        public Interface intf;
        public Interface.Call call;
        public InterfaceException(string message, Interface intf = null, Interface.Call call = null) : base(message) {
            this.intf = intf;
            this.call = call;
        }
    }

    public class ConsoleInterface : Interface
    {
        public ConsoleInterface() : base("console") {

        }

        public Value write(Dictionary<string, Value> parameters) {
            Console.Write(parameters["value"].getValue());
            return null;
        }

        public override void load() {
            define(new Call("write", new string[] { "value" }, write));
        }
    }

    public class FileInterface : Interface
    {
        public FileInterface() : base("file") {

        }

        public Value exists(Dictionary<string, Value> parameters) {
            return new Value(File.Exists(parameters["file"].str()), ValueType.BOOLEAN);
        }

        public Value read(Dictionary<string, Value> parameters) {
            if (!File.Exists(parameters["file"].str())) throw new Exception("could not find file '" + parameters["file"] + "'");
            else return new Value(File.ReadAllText(parameters["file"].str()), ValueType.STRING);
        }

        public Value write(Dictionary<string, Value> parameters) {
            string content;
            if (parameters["content"].getValue() == null) content = "";
            else content = parameters["content"].str();
            File.WriteAllText(parameters["file"].str(), content);
            return Value.TRUE;
        }

        public override void load() {
            define(new Call("read", new string[] { "file" }, read));
            define(new Call("write", new string[] { "file", "content" }, write));
            define(new Call("exists", new string[] { "file" }, exists));
        }
    }
}
