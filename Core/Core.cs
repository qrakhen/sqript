using System;

namespace Qrakhen.Sqript
{

	public enum Access {
		PRIVATE = 0x1,
		PROTECTED = 0x2,
		INTERNAL = 0x4,
		PUBLIC = 0x8
	}

	public static class Core 
	{

		internal static void Init() {
			DefineKeywords();
			DefineOperators();
			DefineNativeCalls();
		}

		internal static void DefineNativeCalls() {
			Native.Define(
				ValueType.Any,
				"toString",
				new Func<QValue, QValue[], QValue>(delegate (QValue target, QValue[] parameters) {
					return new QValue(Operator.GetRealValue(target).ToString(), ValueType.String);
				}
			));
			Native.Define(
				ValueType.Any,
				"getType",
				new Func<QValue, QValue[], QValue>(delegate (QValue target, QValue[] parameters) {
					return new QValue(Operator.GetRealValue(target).GetType(), ValueType.String);
				}
			));
			Native.Define(
				ValueType.Any,
				"equals",
				new Func<QValue, QValue[], QValue>(delegate (QValue target, QValue[] parameters) {
					QValue compare = parameters.Length > 0 ? parameters[0] : null;
					if (compare == null) {
						return QValue.False;
					}
					return target.Equals(compare) ? QValue.True : QValue.False;
				}
			));
		}

		internal static void DefineKeywords() {
			// ToDo: continue, break
			// ToDo: Copy Array by value
			// ToDo: try/catch
			Keywords.Define(Keyword.REFERENCE, "reference", "declare", "var", "ref", "*~");
			Keywords.Define(Keyword.DESTROY, "destroy", "dereference", "del", ":~");
			Keywords.Define(Keyword.NEW, "create", "new", "spawn", "~*");
			Keywords.Define(Keyword.QLASS, "qlass", "class");
			Keywords.Define(Keyword.FUNQTION, "funqtion", "fq", "function", "func", "funq");
			Keywords.Define(Keyword.RETURN, "return", "rightBackAtYou", "back");
			Keywords.Define(Keyword.CURRENT_CONTEXT, "this", "self", ".~");
			Keywords.Define(Keyword.PARENT_CONTEXT, "parent", "^~");
			Keywords.Define(Keyword.CONDITION_IF, "if", "when", "~?");
			Keywords.Define(Keyword.CONDITION_ELSE, "else", "otherwise", "?~");
			Keywords.Define(Keyword.CONDITION_LOOP, "do", "while", "for", "loop", "repeat", "until");
			Keywords.Define(Keyword.BOOL_TRUE, "true");
			Keywords.Define(Keyword.BOOL_FALSE, "false");
		}

		internal static void DefineOperators() {

			Operators.Define(Operator.ASSIGN_VALUE, delegate (QValue a, QValue b) {
				if (a == null && b != null) {
					// return statement
					return b;
				}
				if (!(a is Reference))
					throw new OperationException("can not assign value to a non-reference.");
				Log.Spam("assigning value '" + b + "' to reference '" + a + "'.");
				(a as Reference).Assign(b);
				return a;
			});

			Operators.Define(Operator.ASSIGN_REFERENCE, delegate (QValue a, QValue b) {
				if (a == null && b != null) {
					// return statement
					return b is Reference ? b : new Reference(b);
				}
				if (!(a is Reference))
					throw new OperationException("can not assign value to a non-reference.");
				Log.Spam("referencing the value of '" + b + "' to reference '" + a + "'.");
				if (b is Reference) {
					(a as Reference).Assign(b);
				} else {
					(a as Reference).Assign(new Reference(b));
				}
				return a;
			});

			Operators.Define(Operator.CALCULATE_ADD, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Decimal) || b.IsType(ValueType.Decimal)) {
					return new QValue(Convert.ToDecimal(a.GetValue()) + Convert.ToDecimal(b.GetValue()), ValueType.Decimal);
				} else if (a.IsType(ValueType.String) || b.IsType(ValueType.String)) {
					return new QValue(a.Str() + b.Str(), ValueType.String);
				} else if (a.IsType(ValueType.Integer) && b.IsType(ValueType.Integer)) {
					return new QValue(a.GetValue<int>() + b.GetValue<int>(), ValueType.Integer);
				} else {
					throw new OperationException("can not add values of types " + a.Type.ToString() + " and " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.CALCULATE_SUBTRACT, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Decimal) || b.IsType(ValueType.Decimal)) {
					return new QValue(Convert.ToDecimal(a.GetValue()) - Convert.ToDecimal(b.GetValue()), ValueType.Decimal);
				} else if (a.IsType(ValueType.Integer) && b.IsType(ValueType.Integer)) {
					return new QValue(a.GetValue<int>() - b.GetValue<int>(), ValueType.Integer);
				} else {
					throw new OperationException("can not substract values of types " + a.Type.ToString() + " and " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.CALCULATE_DIVIDE, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Decimal) || b.IsType(ValueType.Decimal)) {
					return new QValue(Convert.ToDecimal(a.GetValue()) / Convert.ToDecimal(b.GetValue()), ValueType.Decimal);
				} else if (a.IsType(ValueType.Integer) && b.IsType(ValueType.Integer)) {
					return new QValue(a.GetValue<int>() / b.GetValue<int>(), ValueType.Integer);
				} else {
					throw new OperationException("can not divide values of types " + a.Type.ToString() + " and " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.CALCULATE_MULTIPLY, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Decimal) || b.IsType(ValueType.Decimal)) {
					return new QValue(Convert.ToDecimal(a.GetValue()) * Convert.ToDecimal(b.GetValue()), ValueType.Decimal);
				} else if (a.IsType(ValueType.Integer) && b.IsType(ValueType.Integer)) {
					return new QValue(a.GetValue<int>() * b.GetValue<int>(), ValueType.Integer);
				} else {
					throw new OperationException("can not multiply values of types " + a.Type.ToString() + " and " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.CONDITION_AND, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Boolean) && b.IsType(ValueType.Boolean)) {
					return new QValue(a.GetValue<bool>() && b.GetValue<bool>(), ValueType.Boolean);
				} else {
					throw new OperationException("can not compare types for " + a.Type.ToString() + " AND " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.CONDITION_OR, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Boolean) && b.IsType(ValueType.Boolean)) {
					return new QValue(a.GetValue<bool>() || b.GetValue<bool>(), ValueType.Boolean);
				} else {
					throw new OperationException("can not compare types for " + a.Type.ToString() + " OR " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.CONDITION_EQUALS, delegate (QValue a, QValue b) {
				while (a.IsType(ValueType.Identifier) || a.IsType(ValueType.Reference))
					a = (QValue) a.GetValue();
				while (b.IsType(ValueType.Identifier) || b.IsType(ValueType.Reference))
					b = (QValue) b.GetValue();
				return new QValue(a.GetValue().Equals(b.GetValue()), ValueType.Boolean);
			});

			Operators.Define(Operator.CONDITION_NOT_EQUALS, delegate (QValue a, QValue b) {
				while (a.IsType(ValueType.Identifier) || a.IsType(ValueType.Reference))
					a = (QValue) a.GetValue();
				while (b.IsType(ValueType.Identifier) || b.IsType(ValueType.Reference))
					b = (QValue) b.GetValue();
				return new QValue(!a.GetValue().Equals(b.GetValue()), ValueType.Boolean);
			});

			Operators.Define(Operator.CONDITION_SMALLER, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Decimal) || b.IsType(ValueType.Decimal)) {
					return new QValue((Convert.ToDecimal(a.GetValue()) < Convert.ToDecimal(b.GetValue())), ValueType.Boolean);
				} else if (a.IsType(ValueType.Integer) && b.IsType(ValueType.Integer)) {
					return new QValue((a.GetValue<int>() < b.GetValue<int>()), ValueType.Boolean);
				} else {
					throw new OperationException("can not compare types for " + a.Type.ToString() + " SMALLER " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.CONDITION_SMALLER_EQUAL, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Decimal) || b.IsType(ValueType.Decimal)) {
					return new QValue((Convert.ToDecimal(a.GetValue()) <= Convert.ToDecimal(b.GetValue())), ValueType.Boolean);
				} else if (a.IsType(ValueType.Integer) && b.IsType(ValueType.Integer)) {
					return new QValue((a.GetValue<int>() <= b.GetValue<int>()), ValueType.Boolean);
				} else {
					throw new OperationException("can not compare types for " + a.Type.ToString() + " SMALLER_EQUAL " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.CONDITION_BIGGER, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Decimal) || b.IsType(ValueType.Decimal)) {
					return new QValue((Convert.ToDecimal(a.GetValue()) > Convert.ToDecimal(b.GetValue())), ValueType.Boolean);
				} else if (a.IsType(ValueType.Integer) && b.IsType(ValueType.Integer)) {
					return new QValue((a.GetValue<int>() > b.GetValue<int>()), ValueType.Boolean);
				} else {
					throw new OperationException("can not compare types for " + a.Type.ToString() + " BIGGER " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.CONDITION_BIGGER_EQUAL, delegate (QValue a, QValue b) {
				a = Operator.GetRealValue(a);
				b = Operator.GetRealValue(b);
				if (a.IsType(ValueType.Decimal) || b.IsType(ValueType.Decimal)) {
					return new QValue((Convert.ToDecimal(a.GetValue()) >= Convert.ToDecimal(b.GetValue())), ValueType.Boolean);
				} else if (a.IsType(ValueType.Integer) && b.IsType(ValueType.Integer)) {
					return new QValue((a.GetValue<int>() >= b.GetValue<int>()), ValueType.Boolean);
				} else {
					throw new OperationException("can not compare types for " + a.Type.ToString() + " BIGGER_EQUAL " + b.Type.ToString());
				}
			});

			Operators.Define(Operator.COLLECTION_ADD_LEFT);
			Operators.Define(Operator.COLLECTION_ADD_RIGHT);
			Operators.Define(Operator.COLLECTION_REMOVE_LEFT);
			Operators.Define(Operator.COLLECTION_REMOVE_RIGHT);
		}
	}
}
