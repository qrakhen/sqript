using System;

namespace Qrakhen.Sqript
{
    public enum Access
    {
        PRIVATE = 0x1,
        PROTECTED = 0x2,
        INTERNAL = 0x4,
        PUBLIC = 0x8
    }

    public static class Core
    {
        internal static void init() {
            defineKeywords();
            defineOperators();
            defineNativeCalls();
        }

        internal static void defineNativeCalls() {
            Native.define(
                ValueType.Any, 
                "toString", 
                new Func<Value, Value[], Value>(delegate (Value target, Value[] parameters) {
                    return new Value(Operator.getRealValue(target).ToString(), ValueType.String);
                }
            ));
            Native.define(
                ValueType.Any,
                "getType",
                new Func<Value, Value[], Value>(delegate (Value target, Value[] parameters) {
                    return new Value(Operator.getRealValue(target).GetType(), ValueType.String);
                }
            ));
            Native.define(
                ValueType.Any,
                "equals",
                new Func<Value, Value[], Value>(delegate (Value target, Value[] parameters) {
                    Value compare = parameters.Length > 0 ? parameters[0] : null;
                    if (compare == null) return Value.False;
                    return target.Equals(compare) ? Value.True : Value.False;
                }
            ));
        }

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
            Keywords.define(Keyword.CONDITION_LOOP, "do", "while", "for", "loop", "repeat", "until");
            Keywords.define(Keyword.BOOL_TRUE, "true");
            Keywords.define(Keyword.BOOL_FALSE, "false");
        }

        internal static void defineOperators() {

            Operators.define(Operator.ASSIGN_VALUE, delegate (Value a, Value b) {
                if (a == null && b != null) {
                    // return statement
                    return b;
                }
                if (!(a is Reference)) throw new OperationException("can not assign value to a non-reference.");
                Log.spam("assigning value '" + b + "' to reference '" + a + "'.");
                (a as Reference).assign(b);
                return a;
            });

            Operators.define(Operator.ASSIGN_REFERENCE, delegate (Value a, Value b) {
                if (a == null && b != null) {
                    // return statement
                    if (b is Reference) return b;
                    else return new Reference(b);
                }
                if (!(a is Reference)) throw new OperationException("can not assign value to a non-reference.");
                Log.spam("referencing the value of '" + b + "' to reference '" + a + "'.");
                if (b is Reference) (a as Reference).assign(b);
                else (a as Reference).assign(new Reference(b));
                return a;
            });

            Operators.define(Operator.CALCULATE_ADD, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Decimal) || b.isType(ValueType.Decimal)) {
                    return new Value(Convert.ToDecimal(a.getValue()) + Convert.ToDecimal(b.getValue()), ValueType.Decimal);
                } else if (a.isType(ValueType.String) || b.isType(ValueType.String)) {
                    return new Value(a.str() + b.str(), ValueType.String);
                } else if (a.isType(ValueType.Integer) && b.isType(ValueType.Integer)) {
                    return new Value(a.getValue<int>() + b.getValue<int>(), ValueType.Integer);
                } else throw new OperationException("can not add values of types " + a.type.ToString() + " and " + b.type.ToString());
            });

            Operators.define(Operator.CALCULATE_SUBTRACT, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Decimal) || b.isType(ValueType.Decimal)) {
                    return new Value(Convert.ToDecimal(a.getValue()) - Convert.ToDecimal(b.getValue()), ValueType.Decimal);
                } else if (a.isType(ValueType.Integer) && b.isType(ValueType.Integer)) {
                    return new Value(a.getValue<int>() - b.getValue<int>(), ValueType.Integer);
                } else throw new OperationException("can not substract values of types " + a.type.ToString() + " and " + b.type.ToString());
            });

            Operators.define(Operator.CALCULATE_DIVIDE, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Decimal) || b.isType(ValueType.Decimal)) {
                    return new Value(Convert.ToDecimal(a.getValue()) / Convert.ToDecimal(b.getValue()), ValueType.Decimal);
                } else if (a.isType(ValueType.Integer) && b.isType(ValueType.Integer)) {
                    return new Value(a.getValue<int>() / b.getValue<int>(), ValueType.Integer);
                } else throw new OperationException("can not divide values of types " + a.type.ToString() + " and " + b.type.ToString());
            });

            Operators.define(Operator.CALCULATE_MULTIPLY, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Decimal) || b.isType(ValueType.Decimal)) {
                    return new Value(Convert.ToDecimal(a.getValue()) * Convert.ToDecimal(b.getValue()), ValueType.Decimal);
                } else if (a.isType(ValueType.Integer) && b.isType(ValueType.Integer)) {
                    return new Value(a.getValue<int>() * b.getValue<int>(), ValueType.Integer);
                } else throw new OperationException("can not multiply values of types " + a.type.ToString() + " and " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_AND, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Boolean) && b.isType(ValueType.Boolean)) {
                    return new Value(a.getValue<bool>() && b.getValue<bool>(), ValueType.Boolean);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " AND " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_OR, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Boolean) && b.isType(ValueType.Boolean)) {
                    return new Value(a.getValue<bool>() || b.getValue<bool>(), ValueType.Boolean);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " OR " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_EQUALS, delegate (Value a, Value b) {
                return new Value(a.getValue().Equals(b.getValue()), ValueType.Boolean);
            });

            Operators.define(Operator.CONDITION_SMALLER, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Decimal) || b.isType(ValueType.Decimal)) {
                    return new Value((Convert.ToDecimal(a.getValue()) < Convert.ToDecimal(b.getValue())), ValueType.Boolean);
                } else if (a.isType(ValueType.Integer) && b.isType(ValueType.Integer)) {
                    return new Value((a.getValue<int>() < b.getValue<int>()), ValueType.Boolean);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " SMALLER " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_SMALLER_EQUAL, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Decimal) || b.isType(ValueType.Decimal)) {
                    return new Value((Convert.ToDecimal(a.getValue()) <= Convert.ToDecimal(b.getValue())), ValueType.Boolean);
                } else if (a.isType(ValueType.Integer) && b.isType(ValueType.Integer)) {
                    return new Value((a.getValue<int>() <= b.getValue<int>()), ValueType.Boolean);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " SMALLER_EQUAL " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_BIGGER, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Decimal) || b.isType(ValueType.Decimal)) {
                    return new Value((Convert.ToDecimal(a.getValue()) > Convert.ToDecimal(b.getValue())), ValueType.Boolean);
                } else if (a.isType(ValueType.Integer) && b.isType(ValueType.Integer)) {
                    return new Value((a.getValue<int>() > b.getValue<int>()), ValueType.Boolean);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " BIGGER " + b.type.ToString());
            });

            Operators.define(Operator.CONDITION_BIGGER_EQUAL, delegate (Value a, Value b) {
                a = Operator.getRealValue(a);
                b = Operator.getRealValue(b);
                if (a.isType(ValueType.Decimal) || b.isType(ValueType.Decimal)) {
                    return new Value((Convert.ToDecimal(a.getValue()) >= Convert.ToDecimal(b.getValue())), ValueType.Boolean);
                } else if (a.isType(ValueType.Integer) && b.isType(ValueType.Integer)) {
                    return new Value((a.getValue<int>() >= b.getValue<int>()), ValueType.Boolean);
                } else throw new OperationException("can not compare types for " + a.type.ToString() + " BIGGER_EQUAL " + b.type.ToString());
            });

            Operators.define(Operator.COLLECTION_ADD_LEFT);
            Operators.define(Operator.COLLECTION_ADD_RIGHT);
            Operators.define(Operator.COLLECTION_REMOVE_LEFT);
            Operators.define(Operator.COLLECTION_REMOVE_RIGHT);
        }
    }
}
