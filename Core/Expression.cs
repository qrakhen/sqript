using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Expression
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

        private Value recursiveCast(object value) {
            if (value == null) throw new OperationException("value to be casted is null");
            if (value.GetType() == typeof(Value) || value.GetType() == typeof(Token)) {
                if ((value as Value).type == ValueType.IDENTIFIER) return context.getReference((value as Value).getValue<string>()).getReference();
                else return (value as Value);
            } else if (value.GetType() == typeof(Reference)) {
                return ((value as Reference).getReference() as Value);
            } else if (value.GetType() == typeof(Expression)) {
                return recursiveCast((value as Expression).execute());
            } else throw new OperationException("unkown value type provided: " + value.GetType().FullName);
        }

        public Value execute() {
            Value result = null;
            Value
                l = recursiveCast(left),
                r = recursiveCast(right);
            if (left.GetType() == typeof(Reference)) Debug.spam("operation.execute() { " + (left as Reference).name + " " + op.symbol + " " + r.getValue() + " }");
            else Debug.spam("operation.execute() { " + l.getValue() + " " + op.symbol + " " + r.getValue() + " } ");
            switch (op.symbol) {
                case Operator.ASSIGN_VALUE:
                    if (left.GetType() != typeof(Reference)) throw new OperationException("only references as left-hand values allowed for assignment");
                    (left as Reference).assign(r, false);
                    result = r;
                    break;

                case Operator.ASSIGN_REFERENCE:
                    if (left.GetType() != typeof(Reference)) throw new OperationException("only references as left-hand values allowed for assignment");
                    (left as Reference).assign(r, true);
                    result = r;
                    break;

                case Operator.CALCULATE_ADD:
                    if (!l.isType((int)ValueType.NUMBER | (int)ValueType.STRING)) throw new OperationException("can not add value of type " + l.type);
                    if (!r.isType((int)ValueType.NUMBER)) throw new OperationException("can not add value of type " + r.type);
                    if (l.getValueSystemType() == typeof(string)) {
                        result = new Value(ValueType.STRING, l.getValue<string>() + r.getValue().ToString());
                    } else if (l.getValueSystemType() == typeof(decimal) || l.getValueSystemType() == typeof(decimal)) {
                        result = new Value(ValueType.DECIMAL, Convert.ToDecimal(l.getValue()) + Convert.ToDecimal(r.getValue()));
                    } else {
                        result = new Value(ValueType.INTEGER, l.getValue<int>() + r.getValue<int>());
                    } 
                    break;

                case Operator.CALCULATE_SUBTRACT:
                    if (!l.isType((int)ValueType.NUMBER)) throw new OperationException("can not subtract value of type " + l.type);
                    if (!r.isType((int)ValueType.NUMBER)) throw new OperationException("can not subtract value of type " + r.type);
                    if (l.getValueSystemType() == typeof(decimal) || l.getValueSystemType() == typeof(decimal)) {
                        result = new Value(ValueType.DECIMAL, Convert.ToDecimal(l.getValue()) - Convert.ToDecimal(r.getValue()));
                    } else {
                        result = new Value(ValueType.INTEGER, l.getValue<int>() - r.getValue<int>());
                    }
                    break;

                case Operator.CALCULATE_MULTIPLY:
                    if (!l.isType((int)ValueType.NUMBER)) throw new OperationException("can not multiply value of type " + l.type);
                    if (!r.isType((int)ValueType.NUMBER)) throw new OperationException("can not multiply value of type " + r.type);
                    if (l.getValueSystemType() == typeof(decimal) || l.getValueSystemType() == typeof(decimal)) {
                        result = new Value(ValueType.DECIMAL, Convert.ToDecimal(l.getValue()) * Convert.ToDecimal(r.getValue()));
                    } else {
                        result = new Value(ValueType.INTEGER, l.getValue<int>() * r.getValue<int>());
                    }
                    break;

                case Operator.CALCULATE_DIVIDE:
                    if (!l.isType((int)ValueType.NUMBER)) throw new OperationException("can not divide value of type " + l.type);
                    if (!r.isType((int)ValueType.NUMBER)) throw new OperationException("can not divide value of type " + r.type);
                    if (l.getValueSystemType() == typeof(decimal) || l.getValueSystemType() == typeof(decimal)) {
                        if (Convert.ToDecimal(r.getValue()) == 0) throw new OperationException("division by zero");
                        result = new Value(ValueType.DECIMAL, Convert.ToDecimal(l.getValue()) / Convert.ToDecimal(r.getValue()));
                    } else {
                        if (r.getValue<int>() == 0) throw new OperationException("division by zero");
                        result = new Value(ValueType.INTEGER, l.getValue<int>() / r.getValue<int>());
                    }
                    break;

                case Operator.CONDITION_EQUALS:
                    result = new Value(ValueType.BOOLEAN, r.getValue().Equals(l.getValue()));
                    break;

                case Operator.CONDITION_AND:
                    if (!l.isType((int)ValueType.BOOLEAN)) throw new OperationException("can not compare non boolean " + l.type);
                    if (!r.isType((int)ValueType.BOOLEAN)) throw new OperationException("can not compare non boolean " + r.type);
                    result = new Value(ValueType.BOOLEAN, l.getValue<bool>() && r.getValue<bool>());
                    break;

                case Operator.CONDITION_OR:
                    if (!l.isType((int)ValueType.BOOLEAN)) throw new OperationException("can not compare non boolean " + l.type);
                    if (!r.isType((int)ValueType.BOOLEAN)) throw new OperationException("can not compare non boolean " + r.type);
                    result = new Value(ValueType.BOOLEAN, l.getValue<bool>() || r.getValue<bool>());
                    break;
            }
            return result;
        }
    }
}
