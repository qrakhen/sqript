using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public static class Core
    {
        internal static void defineKeywords() {
            Keywords.define(Keyword.REFERENCE, "reference", "declare", "var", "ref", "*~");
            Keywords.define(Keyword.DESTROY, "destroy", "dereference", "del", ":~");
            Keywords.define(Keyword.NEW, "create", "new", "spawn", "~*");
            Keywords.define(Keyword.QLASS, "qlass", "class");
            Keywords.define(Keyword.FUNQTION, "funqtion", "fq", "function", "func");
            Keywords.define(Keyword.RETURN, "return", "rightBackAtYou");
            Keywords.define(Keyword.CURRENT_CONTEXT, "this", "self", ".~");
            Keywords.define(Keyword.PARENT_CONTEXT, "parent", "^~");
            Keywords.define(Keyword.CONDITION_IF, "if", "when", "~?");
            Keywords.define(Keyword.CONDITION_ELSE, "else", "otherwise", "?~");
            Keywords.define(Keyword.CONDITION_LOOP, "do", "while", "for", "loop", "repeat");
        }

        internal static void defineOperators() {

            Operators.define(Operator.ASSIGN_VALUE, delegate (Value a, Value b) {
                if (!(a is Reference)) throw new OperationException("can not assign value to a non-reference.");
                Log.spam("assigning value '" + b + "' to reference '" + a + "'.");
                (a as Reference).assign(b);
                return a;
            });

            Operators.define(Operator.ASSIGN_REFERENCE, delegate (Value a, Value b) {
                if (!(a is Reference)) throw new OperationException("can not assign value to a non-reference.");
                Log.spam("referencing the value of '" + b + "' to reference '" + a + "'.");
                if (b is Reference) (a as Reference).assign(b);
                else (a as Reference).assign(new Reference(b));
                return a;
            });

            Operators.define(Operator.CALCULATE_ADD, delegate (Value a, Value b) {
                if (a.isType(ValueType.DECIMAL) || b.isType(ValueType.DECIMAL)) {
                    return new Value(Convert.ToDecimal(a.getValue()) + Convert.ToDecimal(b.getValue()), ValueType.DECIMAL);
                } else if (a.isType(ValueType.STRING) || b.isType(ValueType.STRING)) {
                    return new Value(a.str() + b.str(), ValueType.STRING);
                } else if (a.isType(ValueType.INTEGER) && b.isType(ValueType.INTEGER)) {
                    return new Value(a.getValue<int>() + b.getValue<int>(), ValueType.INTEGER);
                } else throw new OperationException("can not add values of types " + a.type.ToString() + " and " + b.type.ToString());
            });

            Operators.define(Operator.CALCULATE_SUBTRACT, delegate (Value a, Value b) {
                if (a.isType(ValueType.DECIMAL) || b.isType(ValueType.DECIMAL)) {
                    return new Value(Convert.ToDecimal(a.getValue()) - Convert.ToDecimal(b.getValue()), ValueType.DECIMAL);
                } else if (a.isType(ValueType.INTEGER) && b.isType(ValueType.INTEGER)) {
                    return new Value(a.getValue<int>() - b.getValue<int>(), ValueType.INTEGER);
                } else throw new OperationException("can not substract values of types " + a.type.ToString() + " and " + b.type.ToString());
            });

            Operators.define(Operator.CALCULATE_DIVIDE, delegate (Value a, Value b) {
                if (a.isType(ValueType.DECIMAL) || b.isType(ValueType.DECIMAL)) {
                    return new Value(Convert.ToDecimal(a.getValue()) / Convert.ToDecimal(b.getValue()), ValueType.DECIMAL);
                } else if (a.isType(ValueType.INTEGER) && b.isType(ValueType.INTEGER)) {
                    return new Value(a.getValue<int>() / b.getValue<int>(), ValueType.INTEGER);
                } else throw new OperationException("can not divide values of types " + a.type.ToString() + " and " + b.type.ToString());
            });

            Operators.define(Operator.CALCULATE_MULTIPLY, delegate (Value a, Value b) {
                if (a.isType(ValueType.DECIMAL) || b.isType(ValueType.DECIMAL)) {
                    return new Value(Convert.ToDecimal(a.getValue()) * Convert.ToDecimal(b.getValue()), ValueType.DECIMAL);
                } else if (a.isType(ValueType.INTEGER) && b.isType(ValueType.INTEGER)) {
                    return new Value(a.getValue<int>() * b.getValue<int>(), ValueType.INTEGER);
                } else throw new OperationException("can not multiply values of types " + a.type.ToString() + " and " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_AND, delegate (Value a, Value b) {
                if (a.isType(ValueType.BOOLEAN) && b.isType(ValueType.BOOLEAN)) {
                    return new Value(a.getValue<bool>() && b.getValue<bool>(), ValueType.BOOLEAN);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " AND " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_OR, delegate (Value a, Value b) {
                if (a.isType(ValueType.BOOLEAN) && b.isType(ValueType.BOOLEAN)) {
                    return new Value(a.getValue<bool>() || b.getValue<bool>(), ValueType.BOOLEAN);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " OR " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_EQUALS, delegate (Value a, Value b) {
                return new Value(a.getValue().Equals(b.getValue()), ValueType.BOOLEAN);
            });

            Operators.define(Operator.CONDITION_SMALLER, delegate (Value a, Value b) {
                if (a.isType(ValueType.DECIMAL) || b.isType(ValueType.DECIMAL)) {
                    return new Value((Convert.ToDecimal(a.getValue()) < Convert.ToDecimal(b.getValue())), ValueType.BOOLEAN);
                } else if (a.isType(ValueType.INTEGER) && b.isType(ValueType.INTEGER)) {
                    return new Value((a.getValue<int>() < b.getValue<int>()), ValueType.BOOLEAN);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " SMALLER " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_SMALLER_EQUAL, delegate (Value a, Value b) {
                if (a.isType(ValueType.DECIMAL) || b.isType(ValueType.DECIMAL)) {
                    return new Value((Convert.ToDecimal(a.getValue()) <= Convert.ToDecimal(b.getValue())), ValueType.BOOLEAN);
                } else if (a.isType(ValueType.INTEGER) && b.isType(ValueType.INTEGER)) {
                    return new Value((a.getValue<int>() <= b.getValue<int>()), ValueType.BOOLEAN);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " SMALLER_EQUAL " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_BIGGER, delegate (Value a, Value b) {
                if (a.isType(ValueType.DECIMAL) || b.isType(ValueType.DECIMAL)) {
                    return new Value((Convert.ToDecimal(a.getValue()) > Convert.ToDecimal(b.getValue())), ValueType.BOOLEAN);
                } else if (a.isType(ValueType.INTEGER) && b.isType(ValueType.INTEGER)) {
                    return new Value((a.getValue<int>() > b.getValue<int>()), ValueType.BOOLEAN);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " BIGGER " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_BIGGER_EQUAL, delegate (Value a, Value b) {
                if (a.isType(ValueType.DECIMAL) || b.isType(ValueType.DECIMAL)) {
                    return new Value((Convert.ToDecimal(a.getValue()) >= Convert.ToDecimal(b.getValue())), ValueType.BOOLEAN);
                } else if (a.isType(ValueType.INTEGER) && b.isType(ValueType.INTEGER)) {
                    return new Value((a.getValue<int>() >= b.getValue<int>()), ValueType.BOOLEAN);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " BIGGER_EQUAL " + b.type.ToString());
            });

            Operators.define(Operator.COLLECTION_ADD);
            Operators.define(Operator.COLLECTION_REMOVE);
        }
    }
}
