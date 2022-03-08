using System.Collections.Generic;

namespace Qrakhen.Sqript {

	internal class Funqtionizer : Interpretoken {

		public Funqtionizer(Token[] stack) : base(stack) { }


		public static Funqtion Parse(Qontext context, Token[] stack) {
			return new Funqtionizer(stack).Parse(context);
		}

		public Funqtion Parse(Qontext context) {
			Token t = Digest();
			if (t.Check(Struqture.Funqtion[OPEN])) {
				Funqtion fq = new Funqtion(context);
				do {
					t = Peek();
					if (t.Check(Struqture.Context[OPEN])) {
						break;
					} else if (t.Check(ValueType.Identifier)) {
						fq.Parameters.Add(t.Str());
						Digest();
					} else {
						throw new ParseException("unexpected token found when trying to parse funqtion parameter declaration", t);
					}
				} while (!EndOfStack());
				if (EndOfStack())
					throw new ParseException("unexpected end of stack when trying to parse funqtion parameter declaration", t);
				else {
					Token[] body = ReadBody();
					fq.Segments.AddRange(new Segmentizer(body).Parse(fq));
					if (Peek().Check(Struqture.Funqtion[CLOSE])) {
						return fq;
					} else if (Peek().Check(Struqture.Funqtion[DEL])) {
						throw new FunqtionizerException("funqtions overloads not yet implemented", Peek());
					} else {
						throw new ParseException("unexpected token found when trying to parse funqtion body definition", Peek());
					}
				}
			} else {
				throw new ParseException("unexpected funqtion parameter opening, expected '~('", t);
			}
		}

		public QValue[] ParseParameters(Qontext context) {
			List<QValue> parameters = new List<QValue>();
			Token t = Peek();
			if (t.Check(Struqture.Call[OPEN])) {
				do {
					var seg = Segmentizer.ParseOne(context, ReadBody(false, ",)"));
					if (seg == null) {
						return parameters.ToArray();
					}
					parameters.Add(seg.Execute(context));
					Shift(-1);
					if (Peek().Check(Struqture.Call[CLOSE])) {
						break;
					}
					if (Peek().Check(Struqture.Call[DEL])) {
						continue;
					} else {
						throw new ParseException("unexpected token found when trying to parse funqtion call", t);
					}
				} while (!EndOfStack());
				return parameters.ToArray();
			} else {
				throw new ParseException("unexpected funqtion call parameter opening, expected '('", t);
			}
		}

		public static QValue[] ParseParameters(Qontext context, Token[] stack) {
			return new Funqtionizer(stack).ParseParameters(context);
		}
	}


	internal class FunqtionizerException : Exception {

		public FunqtionizerException(string message, Token cause = null) : base(message, cause) { }

	}
}
