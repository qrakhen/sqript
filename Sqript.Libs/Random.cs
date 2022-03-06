using System;
using System.Collections.Generic;
using Qrakhen.Sqript;

namespace Qrakhen.SqriptLib {
	public class RandomInterface : Interface {

		private Random random;

		public RandomInterface() : base("random") {
			random = new Random();
		}

		public Value set_seed(Dictionary<string, Value> parameters) {
			if(!parameters.ContainsKey("seed")) {
				throw new ArgumentException("The needed parameter 'seed' is missing!");
			}
			random = new Random(toInt(parameters, "seed"));
			return null;
		}

		public Value range(Dictionary<string, Value> parameters) {
			if(!parameters.ContainsKey("min")) {
				throw new ArgumentException("The needed parameter 'min' is missing!");
			}
			if(!parameters.ContainsKey("max")) {
				throw new ArgumentException("The needed parameter 'max' is missing!");
			}
			int min = toInt(parameters, "min");
			int max = toInt(parameters, "max");
			return new Value(
				random.Next(min, max + 1),
				Sqript.ValueType.Integer
			);
		}

		public Value rangeD() {
			return new Value(
				random.NextDouble(),
				Sqript.ValueType.Decimal
			);
		}

		public override void load() {
			define(new Call(set_seed, new string[] { "seed" }, Sqript.ValueType.Null));
			define(new Call(range, new string[] { "min", "max" }, Sqript.ValueType.Integer));
			define(new Call(rangeD, Sqript.ValueType.Decimal));
		}

		private int toInt(Dictionary<string, Value> parameters, string name) {
			if(parameters[name].type != Sqript.ValueType.Integer
				&& parameters[name].type != Sqript.ValueType.Decimal
				&& parameters[name].type != Sqript.ValueType.Number
				&& parameters[name].type != Sqript.ValueType.Any
				&& parameters[name].type != Sqript.ValueType.String){
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Integer' but it is: " + parameters[name].type);
			}
			try {
				return int.Parse(parameters[name].value.ToString());
			} catch(FormatException) {
				throw new ArgumentException("The parameter '" + name + "' should have they type 'Integer' but it is: " + parameters[name].type);
			}
		}

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
	}
}
