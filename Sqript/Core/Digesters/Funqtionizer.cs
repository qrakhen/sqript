using System.Collections.Generic;

namespace Qrakhen.Sqript {
	internal class Funqtionizer : Interpretoken {
		public Funqtionizer(Token[] stack) : base(stack) { }

		public static Funqtion parse(Qontext context, Token[] stack) {
			return new Funqtionizer(stack).parse(context);
		}

		public Funqtion parse(Qontext context) {
			Token t = digest();
			if(t.check(Struqture.Funqtion[OPEN])) {
				Funqtion fq = new Funqtion(context);
				do {
					t = peek();
					if(t.check(Struqture.Context[OPEN]))
						break;
					else if(t.check(ValueType.Identifier)) {
						fq.parameters.Add(t.str());
						digest();
					} else
						throw new ParseException("unexpected token found when trying to parse funqtion parameter declaration", t);
				} while(!endOfStack());
				if(endOfStack())
					throw new ParseException("unexpected end of stack when trying to parse funqtion parameter declaration", t);
				else {
					Token[] body = readBody();
					fq.segments.AddRange(new Segmentizer(body).parse(fq));
					if(peek().check(Struqture.Funqtion[CLOSE])) {
						return fq;
					} else if(peek().check(Struqture.Funqtion[DEL])) {
						throw new FunqtionizerException("funqtions overloads not yet implemented", peek());
					} else
						throw new ParseException("unexpected token found when trying to parse funqtion body definition", peek());
				}
			} else
				throw new ParseException("unexpected funqtion parameter opening, expected '~('", t);
		}

		public Value[] parseParameters(Qontext context) {
			List<Value> parameters = new List<Value>();
			Token t = peek();
			if(t.check(Struqture.Call[OPEN])) {
				do {
					var seg = Segmentizer.parseOne(context, readBody(false, ",)"));
					if(seg == null) {
						return parameters.ToArray();
					}
					parameters.Add(seg.execute(context));
					shift(-1);
					if(peek().check(Struqture.Call[CLOSE]))
						break;
					if(peek().check(Struqture.Call[DEL]))
						continue;
					else
						throw new ParseException("unexpected token found when trying to parse funqtion call", t);
				} while(!endOfStack());
				return parameters.ToArray();
			} else
				throw new ParseException("unexpected funqtion call parameter opening, expected '('", t);
		}

		public static Value[] parseParameters(Qontext context, Token[] stack) {
			return new Funqtionizer(stack).parseParameters(context);
		}
	}

	internal class FunqtionizerException : Exception {
		public FunqtionizerException(string message, Token cause = null) : base(message, cause) { }
	}
}
