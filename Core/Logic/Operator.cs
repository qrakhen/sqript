using System;
using System.Collections.Generic;

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

        public string symbol { get; protected set; }
        public Func<Value, Value, Value> execute { get; protected set; }

        public Operator(string symbol, Func<Value, Value, Value> calculate = null) {
            this.symbol = symbol;
            this.execute = calculate;
        }

        public int compare(Operator op)
        {
            return importance - op.importance;
        }

        public int importance => (symbol == CALCULATE_ADD || symbol == CALCULATE_SUBTRACT ? 1 : (symbol == CALCULATE_MULTIPLY || symbol == CALCULATE_DIVIDE ? 2 : 0));

        public static Value getRealValue(Value v) {
            if (v is Reference) return (v as Reference).getTrueValue();
            else return v;
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
