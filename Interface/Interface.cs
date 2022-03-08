using System;
using System.Collections.Generic;

namespace Qrakhen.Sqript {
	/// <summary>
	/// The Interface class is used to create libraries, for example custom networking implementations.
	/// All default Sqript libraries (i.e. sqlib.base.dll) are made by extending this class.
	/// </summary>
	public abstract class Interface {

		public string Name { get; private set; }
		public Dictionary<string, Call> Calls;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public Interface(string name) {
			this.Name = name;
			Calls = new Dictionary<string, Call>();
		}


		/// <summary>
		/// The load method should use define() to register all interface calls.
		/// </summary>
		public abstract void Load();

		/// <summary>
		/// defines an interface Call, see the Call class for more information
		/// </summary>
		/// <param name="call"></param>
		public void Define(Call call) {
			Calls.Add(call.Name, call);
		}

		internal QValue CallMethod(string name, QValue[] parameters) {
			if (this.Calls.ContainsKey(name)) {
				return Calls[name].Execute(parameters);
			} else {
				throw new InterfaceException("could not find interface call '" + name + "'", this);
			}
		}


		public sealed class Call {

			public delegate QValue CallMethodParam(Dictionary<string, QValue> parameters);
			public delegate QValue CallMethod();

			private readonly CallMethodParam _callParam;
			private readonly CallMethod _call;
			public string Name { get; private set; }
			public string[] Parameters { get; private set; }
			public ValueType ReturnType { get; private set; }

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
				this._callParam = call;
				this.Parameters = parameters;
				this.ReturnType = returnType;
				this.Name = name ?? call.Method.Name;
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
				this._call = call;
				this.ReturnType = returnType;
				this.Name = name ?? call.Method.Name;
			}


			public QValue Execute(QValue[] parameters) {
				if (parameters is null || parameters.Length == 0) {
					return Execute();
				}
				Dictionary<string, QValue> provided = new Dictionary<string, QValue>();
				for (int i = 0; i < this.Parameters.Length; i++) {
					if (parameters.Length <= i) {
						provided.Add(this.Parameters[i], null);
					}
					else {
						provided.Add(this.Parameters[i], parameters[i]);
					}
				}
				return _callParam(provided);
			}

			public QValue Execute() {
				return _call();
			}
		}

		internal class Funqtion : Sqript.Funqtion {

			public readonly Call call; 


			public Funqtion(Qontext parent, Call call) : base(parent) {
				this.call = call;
			}


			public override QValue Execute(QValue[] parameters, QValue caller = null) {
				return call.Execute(parameters);
			}

			public override string ToString() {
				string r = "<" + call.ReturnType + ">(";
				if (call.Parameters != null) {
					foreach (string parameter in call.Parameters) {
						r += parameter + ", ";
					}
					if (r.EndsWith(", ")) {
						r = r[0..^2];
					}
				}
				return r + ")";
			}
		}

		internal Obqect CreateInterfaceContext() {
			Obqect context = new Obqect(null, false);
			foreach (var call in Calls) {
				context.Set(call.Key, new Reference(new Funqtion(context, call.Value), context, call.Key, Access.PUBLIC, true));
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
