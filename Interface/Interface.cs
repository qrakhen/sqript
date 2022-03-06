using System;
using System.Collections.Generic;

namespace Qrakhen.Sqript {
	/// <summary>
	/// The Interface class is used to create libraries, for example custom networking implementations.
	/// All default Sqript libraries (i.e. sqlib.base.dll) are made by extending this class.
	/// </summary>
	public abstract class Interface {
		public string name { get; private set; }
		public Dictionary<string, Call> calls;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public Interface(string name) {
			this.name = name;
			calls = new Dictionary<string, Call>();
		}

		/// <summary>
		/// The load method should use define() to register all interface calls.
		/// </summary>
		public abstract void load();

		/// <summary>
		/// defines an interface Call, see the Call class for more information
		/// </summary>
		/// <param name="call"></param>
		public void define(Call call) {
			calls.Add(call.name, call);
		}

		internal Value call(string name, Value[] parameters) {
			if(calls.ContainsKey(name))
				return calls[name].execute(parameters);
			else
				throw new InterfaceException("could not find interface call '" + name + "'", this);
		}

		public sealed class Call {
			public delegate Value CallMethodParam(Dictionary<string, Value> parameters);
			public delegate Value CallMethod();

			public CallMethodParam callParam { get; private set; }
			public CallMethod call { get; private set; }
			public string name { get; private set; }
			public string[] parameters { get; private set; }
			public ValueType returnType { get; private set; }

			/// <summary>
			/// Creates a Method with parameters
			/// </summary>
			/// <param name="call">any function that implements the CallbackMethod delegate (Value(Dictionary<string, Value>))
			/// the actual function, provide any method that accepts a Dictionary as parameter and returns a Sqript.Value
			/// all provided parameters will be accessible using the dictionary (i.e. string key = parameters["key"];)
			/// </param>
			/// <param name="parameters">parameter names to later be able to identify them inside the callback</param>
			/// <param name="returnType">returntype of the function</param>
			/// <param name="name">function name, as used in code</param>
			public Call(CallMethodParam call, string[] parameters, ValueType returnType = ValueType.Any, string name = null) {
				this.callParam = call;
				this.parameters = parameters;
				this.returnType = returnType;
				this.name = name ?? call.Method.Name;
			}

			/// <summary>
			/// Creates a Method with no parameters
			/// </summary>
			/// <param name="call">any function that implements the CallbackMethod delegate (Value(Dictionary<string, Value>))
			/// the actual function, provide any method that accepts a Dictionary as parameter and returns a Sqript.Value
			/// all provided parameters will be accessible using the dictionary (i.e. string key = parameters["key"];)
			/// </param>
			/// <param name="returnType">returntype of the function</param>
			/// <param name="name">function name, as used in code</param>
			public Call(CallMethod call, ValueType returnType = ValueType.Any, string name = null) {
				this.call = call;
				this.returnType = returnType;
				this.name = name ?? call.Method.Name;
			}

			public Value execute(Value[] parameters) {
				if(parameters is null || parameters.Length == 0) {
					return execute();
				}
				Dictionary<string, Value> provided = new Dictionary<string, Value>();
				for(int i = 0; i < this.parameters.Length; i++) {
					if(parameters.Length <= i)
						provided.Add(this.parameters[i], null);
					else
						provided.Add(this.parameters[i], parameters[i]);
				}
				return callParam(provided);
			}

			public Value execute() {
				return call();
			}
		}

		internal class Funqtion : Sqript.Funqtion {
			public Call call { get; private set; }

			public Funqtion(Qontext parent, Call call) : base(parent) {
				this.call = call;
			}

			public override Value execute(Value[] parameters, Value caller = null) {
				return call.execute(parameters);
			}

			public override string ToString() {
				string r = "<" + call.returnType + ">(";
				if(call.parameters != null) {
					foreach(string parameter in call.parameters) {
						r += parameter + ", ";
					}
					if(r.EndsWith(", ")) {
						r = r.Substring(0, r.Length - 2);
					}
				}
				return r + ")";
			}
		}

		internal Obqect createInterfaceContext() {
			Obqect context = new Obqect(null, false);
			foreach(var call in calls) {
				context.set(call.Key, new Reference(new Funqtion(context, call.Value), context, call.Key, Access.PUBLIC, true));
			}
			return context;
		}
	}

	public class InterfaceException : Exception {
		public Interface intf;
		public Interface.Call call;
		public InterfaceException(string message, Interface intf = null, Interface.Call call = null) : base(message) {
			this.intf = intf;
			this.call = call;
		}
	}
}
