using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal class Expression
    {
        private Context context;

        public object left { get; protected set; }
        public object right { get; protected set; }
        public Operator op { get; protected set; }

        public Expression(Operator op, object left, object right, Context context) {
            this.op = op;
            this.left = left;
            this.right = right;
            this.context = context;
        }

        private Value getTrueValue(object value) {
            if (value == null) throw new OperationException("value to be casted is null");
            if (value.GetType() == typeof(Reference)) {
                return ((value as Reference).getReference() as Value);
            } else if (value.GetType() == typeof(Expression)) {
                return getTrueValue((value as Expression).execute());
            } else if (value is Funqtion) {
                return (value as Funqtion).execute();
            } else if (value is Value) {
                if ((value as Value).type == ValueType.IDENTIFIER) return context.lookup((value as Value).getValue<string>()).getReference();
                else return (value as Value);
            } else throw new OperationException("unkown value type provided: " + value.GetType().FullName);
        }

        public Value execute() {
            if (left == null) return null;
            else if (right == null) return getTrueValue(left); // maybe add manipulator

            Value
                l = getTrueValue(left),
                r = getTrueValue(right);
            if (left.GetType() == typeof(Reference)) Log.spam("operation.execute() { " + (left as Reference).getValue() + " " + op.symbol + " " + r.ToString() + " }");
            else Log.spam("operation.execute() { " + l.getValue() + " " + op.symbol + " " + r.getValue() + " } ");

            if (op.execute != null) return op.execute(l, r);
            else throw new Exception("no operator callback found for '" + op.symbol + "'");
        }
    }
}
