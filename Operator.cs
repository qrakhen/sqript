using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Operator
    {
        public const string
            ASSIGN_VALUE = "=",
            ASSIGN_REFERENCE = "<&",
            CALCULATE_ADD = "+",
            CALCULATE_SUBSTRACT = "-",
            CALCULATE_MULTIPLY = "*",
            CALCULATE_DIVIDE = "/",
            CONDITION_AND = "&&",
            CONDITION_OR = "||",
            CONDITION_EQUALS = "==",
            COLLECTION_ADD = "<",
            COLLECTION_REMOVE = ">",
            KEY_DELIMITER = ":";

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

    public class Operation
    {
        private Context context;

        public object left { get; protected set; }
        public object right { get; protected set; }
        public Operator op { get; protected set; }

        public Operation(Operator op, object left, object right, Context context) {
            this.op = op;
            this.left = left;
            this.right = right;
            this.context = context;
        }

        private Value recursiveCast(object value) {
            if (value == null) throw new OperationException("value to be casted is null");
            if (value.GetType() == typeof(Value) || value.GetType() == typeof(Token)) return (value as Value);
            else if (value.GetType() == typeof(Reference)) return ((value as Reference).getValue() as Value);
            else if (value.GetType() == typeof(Operation)) return recursiveCast((value as Operation).execute());
            else throw new OperationException("unkown value type provided: " + value.GetType().FullName);
        }

        public object execute() {
            object result = null;
            Value
                l = recursiveCast(left),
                r = recursiveCast(right);
            SqriptDebug.spam("operation.execute() { " + (l == null ? "undefined" : l.getValue()) + " " + op.symbol + " " + (r == null ? "undefined" : r.getValue()) + " } ");
            switch (op.symbol) {
                case Operator.ASSIGN_VALUE:
                    if (left.GetType() != typeof(Reference)) throw new OperationException("only references as left-hand values allowed for assignment");
                    (left as Reference).setValue(r.getValue(), r.type);
                    result = left;
                    break;

                case Operator.CALCULATE_ADD:
                    if (!l.isType((int) Value.Type.NUMBER)) throw new OperationException("only numbers allowed for adding");
                    if (!r.isType((int) Value.Type.NUMBER)) throw new OperationException("only numbers allowed for adding");
                    result = l.getValue<int>() + r.getValue<int>();
                    break;
            }
            return result;
        }
    }
}
