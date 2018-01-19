using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Operator
    {
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
            CONDITION_SMALLER = "<",
            CONDITION_SMALLER_EQUAL = "<=",
            CONDITION_BIGGER = ">",
            CONDITION_BIGGER_EQUAL = ">=",
            COLLECTION_ADD = "<+",
            COLLECTION_REMOVE = "->",
            NO_OPERATION = "";

        public string symbol { get; protected set; }
        public Func<Value, Value, Value> execute { get; protected set; }

        public Operator(string symbol, Func<Value, Value, Value> calculate = null) {
            this.symbol = symbol;
            this.execute = calculate;
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

        public static void define(string symbol, Func<Value, Value, Value> calculate = null) {
            if (get(symbol) != null) throw new InvalidOperationException("operator with given name already exists");
            operators.Add(symbol, new Operator(symbol, calculate));
        }
    }
}
