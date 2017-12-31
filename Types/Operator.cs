using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Operator
    {
        public const string
            ASSIGN_VALUE = "=",
            ASSIGN_REFERENCE = "~&",
            CALCULATE_ADD = "+",
            CALCULATE_SUBTRACT = "-",
            CALCULATE_MULTIPLY = "*",
            CALCULATE_DIVIDE = "/",
            CONDITION_AND = "&&",
            CONDITION_OR = "||",
            CONDITION_EQUALS = "==",
            COLLECTION_ADD = "<+",
            COLLECTION_REMOVE = "~>",
            NO_OPERATION = "";

        public string symbol { get; protected set; }
        public Action<Value, Value, Value> calculate { get; protected set; }

        public Operator(string symbol, Action<Value, Value, Value> calculate) {
            this.symbol = symbol;
            this.calculate = calculate;
        }

        public override string ToString() {
            return symbol;
        }
    }

    public static class Operators
    {
        private static Dictionary<string, Operator> operators = new Dictionary<string, Operator>();

        public static Operator get(string symbol) {
            if (operators.ContainsKey(symbol)) return operators[symbol];
            return null;
        }

        public static void define(string symbol, Action<Value, Value, Value> calculate) {
            if (get(symbol) != null) throw new InvalidOperationException("operator with given name already exists");
            operators.Add(symbol, new Operator(symbol, calculate));
        }
    }
}
