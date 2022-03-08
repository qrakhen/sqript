using System;
using System.Collections.Generic;

namespace Qrakhen.Sqript {

	public class Operator {

		public const string
			ASSIGN_VALUE = "<~",
			ASSIGN_REFERENCE = "<&",
			CALCULATE_ADD = "+",
			CALCULATE_SUBTRACT = "-",
			CALCULATE_MULTIPLY = "*",
			CALCULATE_DIVIDE = "/",
			CONDITION_AND = "&&",
			CONDITION_OR = "||",
			CONDITION_EQUALS = "==",
			CONDITION_NOT_EQUALS = "!=",
			CONDITION_SMALLER = "<",
			CONDITION_SMALLER_EQUAL = "<=",
			CONDITION_BIGGER = ">",
			CONDITION_BIGGER_EQUAL = ">=",
			COLLECTION_ADD_LEFT = "<+",
			COLLECTION_ADD_RIGHT = "+>",
			COLLECTION_REMOVE_LEFT = "->",
			COLLECTION_REMOVE_RIGHT = "<-",
			NO_OPERATION = "";

		// list -> item
		// inventory -> shovel
		// inventory 10:-> shovel
		// inventory <+ diamond
		// chrisInv 100:-> diamond +> daveInv &

		public string Symbol { get; protected set; }
		public Func<QValue, QValue, QValue> Execute { get; protected set; }

		public Operator(string symbol, Func<QValue, QValue, QValue> calculate = null) {
			this.Symbol = symbol;
			this.Execute = calculate;
		}

		public int Compare(Operator op) {
			return Importance - op.Importance;
		}

		public int Importance => (Symbol == CALCULATE_ADD || Symbol == CALCULATE_SUBTRACT ? 1 : (Symbol == CALCULATE_MULTIPLY || Symbol == CALCULATE_DIVIDE ? 2 : 0));

		public static QValue GetRealValue(QValue v) {
			return v is Reference ? (v as Reference).GetTrueValue() : v;
		}

		public override string ToString() {
			return Symbol;
		}
	}

	public static class Operators {

		private static readonly Dictionary<string, Operator> _operators = new Dictionary<string, Operator>();

		public static Operator Get(string symbol) {
			return _operators.ContainsKey(symbol) ? _operators[symbol] : null;
		}

		public static void Define(string symbol, Func<QValue, QValue, QValue> calculate = null) {
			if (Get(symbol) != null)
				throw new InvalidOperationException("operator with given name already exists");
			_operators.Add(symbol, new Operator(symbol, calculate));
		}
	}
}
