namespace Qrakhen.Sqript {

	internal class Colleqtionizer : Interpretoken {

		public Colleqtionizer(Token[] stack) : base(stack) { }


		public static Obqect ReadObqect(Qontext context, Token[] stack) {
			return new Colleqtionizer(stack).ReadObqect(context);
		}

		public Obqect ReadObqect(Qontext context) {
			Log.Spam("Colleqtionizer.readObqect()");
			Obqect o = new Obqect(context);
			if (stack.Length == 0)
				return o;
			Token t = Digest();
			if (t.Check(Struqture.Context[OPEN])) {
				do {
					t = Peek();
					if (t.Check(ValueType.Identifier)) {
						string key = Digest().Str();
						t = Peek();
						if (t.Check(Operator.ASSIGN_REFERENCE) || t.Check(Operator.ASSIGN_VALUE)) {
							//@TODO: maybe use new Segmentizer().parse() here? we can't do { a <~ 5 + 3 } yet...
							Operator op = Operators.Get(Digest().ToString());
							QValue value = ReadNextValue(context);
							if (value == null) {
								throw new ParseException("could not read value for '" + key + "': unreadable or no value.", t);
							}
							if (value is Reference) {
								if (op.Symbol == Operator.ASSIGN_REFERENCE) {
									o.Set(key, new Reference(value));
								} else {
									o.Set(key, new Reference((value as Reference).GetTrueValue()));
								}
							} else {
								o.Set(key, new Reference(value));
							}
							t = Digest();
							if (t.Check(Struqture.Context[DEL])) {
								continue;

							} else if (t.Check(Struqture.Context[CLOSE])) {
								break;
							} else {
								throw new ParseException("unexpected token '" + t + "' when reading obqect body, expected '}' or ','", t);
							}
						} else {
							throw new ParseException("expected assign_value/reference for the next token, got '" + t + "' instead.", t);
						}
					} else if (t.Check(Struqture.Context[CLOSE])) {
						Digest();
						break;
					} else {
						throw new ParseException("expected identifier as obqect key, got '" + t + "' instead.", t);
					}
				} while (!EndOfStack());
			} else {
				throw new ParseException("unexpected token '" + t + "' when reading obqect body, expected '{'", t);
			}
			return o;
		}

		public static Array ReadArray(Qontext context, Token[] stack) {
			return new Colleqtionizer(stack).ReadArray(context);
		}

		public Array ReadArray(Qontext context) {
			Log.Spam("Colleqtionizer.readArray()");
			Array a = new Array();
			if (stack.Length == 0) {
				return a;
			}
			Token t = Digest();
			if (t.Check(Struqture.Collection[OPEN])) {
				do {
					t = Peek();
					int index = -1;
					Operator op = Operators.Get(Operator.ASSIGN_VALUE);
					if (t.Check(ValueType.Integer)) {
						if (Peek(1).Check(Operator.ASSIGN_VALUE) || Peek(1).Check(Operator.ASSIGN_REFERENCE)) {
							index = Digest().GetValue<int>();
							op = Digest().GetValue<Operator>();
						}
					}

					if (t.Check(Struqture.Collection[CLOSE])) {
						Digest();
						break;
					} else {
						QValue value = ReadNextValue(context);
						if (value == null) {
							throw new ParseException("could not read value for array: unreadable or no value.", t);
						}
						if (value is Reference) {
							if (op.Symbol == Operator.ASSIGN_REFERENCE) {
								a.Set(index, new Reference(value));
							} else {
								a.Set(index, new Reference((value as Reference).GetTrueValue()));
							}
						} else {
							a.Set(index, new Reference(value));
						}
						t = Digest();
						if (t.Check(Struqture.Collection[DEL])) {
							continue;
						} else if (t.Check(Struqture.Collection[CLOSE])) {
							break;
						} else {
							throw new ParseException("unexpected token '" + t + "' when reading array body, expected ']' or ','", t);
						}
					}
				} while (!EndOfStack());
			} else {
				throw new ParseException("unexpected token '" + t + "' when reading array body, expected '['", t);
			}
			return a;
		}
	}
}
