using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Operator
    {
        public string symbol { get; protected set; }
        public Action<Value, Value, Value> calculate { get; protected set; }

        public Operator(string symbol, Action<Value, Value, Value> calculate) {
            this.symbol = symbol;
            this.calculate = calculate;
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

    public class Operation
    {
        public Value left { get; protected set; }
        public Value right { get; protected set; }
        public Operator op { get; protected set; }

        public Operation(Operator op, ref Value left, Value right = null) {
            this.op = op;
            this.left = left;
            this.right = right;
        }

        public Value execute() {
            Value result = new Value(Value.Type.UNDEFINED, null);
            op.calculate(left, right, result);
            return result;
        }
    }
}
