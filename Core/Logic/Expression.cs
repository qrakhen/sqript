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
            //} else if (value is Funqtion) {
                //return (value as Funqtion).execute();
            } else if (value is Value) {
                if ((value as Value).type == ValueType.IDENTIFIER) return context.lookup((value as Value).getValue<string>()).getReference();
                else return (value as Value);
            } else throw new OperationException("unkown value type provided: " + value.GetType().FullName);
        }

        public Value execute() {
            Value result = null;

            if (left == null) return null;
            else if (right == null) return getTrueValue(left); // maybe add manipulator

            Value
                l = getTrueValue(left),
                r = getTrueValue(right);
            if (left.GetType() == typeof(Reference)) Debug.spam("operation.execute() { " + (left as Reference).getValue() + " " + op.symbol + " " + r.ToString() + " }");
            else Debug.spam("operation.execute() { " + l.getValue() + " " + op.symbol + " " + r.getValue() + " } ");

            if (op.calculate != null) return op.calculate(l, r);

            switch (op.symbol) {
                case Operator.ASSIGN_VALUE:
                    if (left.GetType() == typeof(Reference)) {
                        (left as Reference).assign(r, false);
                    } else throw new OperationException("only references as left-hand values allowed for assignment");
                    result = r;
                    break;

                case Operator.ASSIGN_REFERENCE:
                    if (left.GetType() == typeof(Reference)) {
                        (left as Reference).assign(r, true);
                    } else throw new OperationException("only references as left-hand values allowed for assignment");
                    result = r;
                    break;

                case Operator.CALCULATE_ADD:
                    if (!l.isType((int)ValueType.NUMBER | (int)ValueType.STRING)) throw new OperationException("can not add value of type " + l.type);
                    if (!r.isType((int)ValueType.NUMBER)) throw new OperationException("can not add value of type " + r.type);
                    if (l.getValueSystemType() == typeof(string)) {
                        result = new Value(l.getValue<string>() + r.getValue().ToString(), ValueType.STRING);
                    } else if (l.getValueSystemType() == typeof(decimal) || l.getValueSystemType() == typeof(decimal)) {
                        result = new Value(Convert.ToDecimal(l.getValue()) + Convert.ToDecimal(r.getValue()), ValueType.DECIMAL);
                    } else {
                        result = new Value(l.getValue<int>() + r.getValue<int>(), ValueType.INTEGER);
                    } 
                    break;

                case Operator.CALCULATE_SUBTRACT:
                    if (!l.isType((int)ValueType.NUMBER)) throw new OperationException("can not subtract value of type " + l.type);
                    if (!r.isType((int)ValueType.NUMBER)) throw new OperationException("can not subtract value of type " + r.type);
                    if (l.getValueSystemType() == typeof(decimal) || l.getValueSystemType() == typeof(decimal)) {
                        result = new Value(Convert.ToDecimal(l.getValue()) - Convert.ToDecimal(r.getValue()), ValueType.DECIMAL);
                    } else {
                        result = new Value(l.getValue<int>() - r.getValue<int>(), ValueType.INTEGER);
                    }
                    break;

                case Operator.CALCULATE_MULTIPLY:
                    if (!l.isType((int)ValueType.NUMBER)) throw new OperationException("can not multiply value of type " + l.type);
                    if (!r.isType((int)ValueType.NUMBER)) throw new OperationException("can not multiply value of type " + r.type);
                    if (l.getValueSystemType() == typeof(decimal) || l.getValueSystemType() == typeof(decimal)) {
                        result = new Value(Convert.ToDecimal(l.getValue()) * Convert.ToDecimal(r.getValue()), ValueType.DECIMAL);
                    } else {
                        result = new Value(l.getValue<int>() * r.getValue<int>(), ValueType.INTEGER);
                    }
                    break;

                case Operator.CALCULATE_DIVIDE:
                    if (!l.isType((int)ValueType.NUMBER)) throw new OperationException("can not divide value of type " + l.type);
                    if (!r.isType((int)ValueType.NUMBER)) throw new OperationException("can not divide value of type " + r.type);
                    if (l.getValueSystemType() == typeof(decimal) || l.getValueSystemType() == typeof(decimal)) {
                        if (Convert.ToDecimal(r.getValue()) == 0) throw new OperationException("division by zero");
                        result = new Value(Convert.ToDecimal(l.getValue()) / Convert.ToDecimal(r.getValue()), ValueType.DECIMAL);
                    } else {
                        if (r.getValue<int>() == 0) throw new OperationException("division by zero");
                        result = new Value(l.getValue<int>() / r.getValue<int>(), ValueType.INTEGER);
                    }
                    break;

                case Operator.CONDITION_EQUALS:
                    result = new Value(r.getValue().Equals(l.getValue()), ValueType.BOOLEAN);
                    break;

                case Operator.CONDITION_AND:
                    if (!l.isType((int)ValueType.BOOLEAN)) throw new OperationException("can not compare non boolean " + l.type);
                    if (!r.isType((int)ValueType.BOOLEAN)) throw new OperationException("can not compare non boolean " + r.type);
                    result = new Value(l.getValue<bool>() && r.getValue<bool>(), ValueType.BOOLEAN);
                    break;

                case Operator.CONDITION_OR:
                    if (!l.isType((int)ValueType.BOOLEAN)) throw new OperationException("can not compare non boolean " + l.type);
                    if (!r.isType((int)ValueType.BOOLEAN)) throw new OperationException("can not compare non boolean " + r.type);
                    result = new Value(l.getValue<bool>() || r.getValue<bool>(), ValueType.BOOLEAN);
                    break;
            }
            return result;
        }
    }
}
