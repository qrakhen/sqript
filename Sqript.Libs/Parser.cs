using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Qrakhen.Sqript;

namespace Qrakhen.SqriptLib {
	public class ParserInterface : Interface {

		private ConsoleColor _consoleColor = ConsoleColor.White;

		public ParserInterface() : base("parser") {

		}

		public override void load() {
			define(new Call(toNumber, new string[] { "value" }, Sqript.ValueType.Number));
			define(new Call(toInt, new string[] { "value" }, Sqript.ValueType.Number));
			define(new Call(toDecimal, new string[] { "value" }, Sqript.ValueType.Number));
			define(new Call(toBool, new string[] { "value" }, Sqript.ValueType.Number));
		}

		public Value toNumber(Dictionary<string, Value> parameters) {
			if(!parameters.ContainsKey("value")) {
				throw new ArgumentException("The needed parameter 'min' is missing!");
			}
			decimal number = toDecimal(parameters, "value");
			return new Value(
				number,
				Sqript.ValueType.Number
			);
		}

		public Value toInt(Dictionary<string, Value> parameters) {
			if(!parameters.ContainsKey("value")) {
				throw new ArgumentException("The needed parameter 'min' is missing!");
			}
			int number = toInt(parameters, "value");
			return new Value(
				number,
				Sqript.ValueType.Integer
			);
		}

		public Value toDecimal(Dictionary<string, Value> parameters) {
			if(!parameters.ContainsKey("value")) {
				throw new ArgumentException("The needed parameter 'min' is missing!");
			}
			double number = toDouble(parameters, "value");
			return new Value(
				number,
				Sqript.ValueType.Decimal
			);
		}

		public Value toBool(Dictionary<string, Value> parameters) {
			if(!parameters.ContainsKey("value")) {
				throw new ArgumentException("The needed parameter 'min' is missing!");
			}
			bool number = toBool(parameters, "value");
			return new Value(
				number,
				Sqript.ValueType.Boolean
			);
		}

		#region Helper

		private decimal toDecimal(Dictionary<string, Value> parameters, string name) {
			if(parameters[name].type != Sqript.ValueType.Integer
				&& parameters[name].type != Sqript.ValueType.Decimal
				&& parameters[name].type != Sqript.ValueType.Number
				&& parameters[name].type != Sqript.ValueType.Any
				&& parameters[name].type != Sqript.ValueType.String) {
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Decimal' but it is: " + parameters[name].type);
			}
			try {
				return decimal.Parse(parameters[name].value.ToString());
			} catch(FormatException) {
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Decimal' but it is: " + parameters[name].type);
			}
		}

		private int toInt(Dictionary<string, Value> parameters, string name) {
			if(parameters[name].type != Sqript.ValueType.Integer
				&& parameters[name].type != Sqript.ValueType.Decimal
				&& parameters[name].type != Sqript.ValueType.Number
				&& parameters[name].type != Sqript.ValueType.Any
				&& parameters[name].type != Sqript.ValueType.String) {
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Integer' but it is: " + parameters[name].type);
			}
			try {
				return int.Parse(parameters[name].value.ToString());
			} catch(FormatException) {
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Integer' but it is: " + parameters[name].type);
			}
		}

		private double toDouble(Dictionary<string, Value> parameters, string name) {
			if(parameters[name].type != Sqript.ValueType.Integer
				&& parameters[name].type != Sqript.ValueType.Decimal
				&& parameters[name].type != Sqript.ValueType.Number
				&& parameters[name].type != Sqript.ValueType.Any
				&& parameters[name].type != Sqript.ValueType.String) {
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Double' but it is: " + parameters[name].type);
			}
			try {
				return double.Parse(parameters[name].value.ToString());
			} catch(FormatException) {
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Double' but it is: " + parameters[name].type);
			}
		}

		private bool toBool(Dictionary<string, Value> parameters, string name) {
			if(parameters[name].type != Sqript.ValueType.Integer
				&& parameters[name].type != Sqript.ValueType.Decimal
				&& parameters[name].type != Sqript.ValueType.Number
				&& parameters[name].type != Sqript.ValueType.Any
				&& parameters[name].type != Sqript.ValueType.String) {
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Boolean' but it is: " + parameters[name].type);
			}
			try {
				return bool.Parse(parameters[name].value.ToString());
			} catch(FormatException) {
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Boolean' but it is: " + parameters[name].type);
			}
		}

		#endregion
	}
}
