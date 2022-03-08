using System;
using System.Collections.Generic;

namespace Qrakhen.Sqript
{

	internal class Interpretoken : Digester<Token>
	{

		protected const int OPEN = 0x0, CLOSE = 0x1, DEL = 0x2;


		public Interpretoken(Token[] stack) : base(stack) { }
		

		protected override Token Digest() {
			Token t = base.Digest();
			Runtime.reader.token = t;
			return t;
		}

		protected override Token Peek() {
			Token t = base.Peek();
			Runtime.reader.token = t;
			return t;
		}

		public Token[] Remaining() {
			List<Token> buffer = new List<Token>();
			while (!EndOfStack()) {
				buffer.Add(Digest());
			}
			return buffer.ToArray();
		}

		public Token[] ReadBody(bool includeBrackets = false, string until = "") {
			List<Token> buffer = new List<Token>();
			int depth = 1;
			if (includeBrackets)
				buffer.Add(Peek());
			string ascend = Digest().Str();
			string descend = "";
			if (until != "") {
				descend = until;
			} else {
				descend = ascend switch {
					"~(" => ")",
					"(" => ")",
					"{" => "}",
					"[" => "]",
					"<" => ">",
					_ => throw new ParseException("could not find closing element for opened '" + ascend + "'", Peek()),
				};
			}

			do {
				string cur = (Peek().Type == ValueType.Struqture ? Peek().GetValue<string>() : "");
				if (!string.IsNullOrEmpty(cur) && descend.IndexOf(cur) > -1) {
					depth--;
				} else if (cur == ascend) {
					depth++;
				} else if (ascend == "~(" && cur == "(") {
					depth++;
				}

				if (depth > 0) {
					buffer.Add(Digest());
				} else if (depth == 0) {
					if (includeBrackets) {
						buffer.Add(Digest());
					} else {
						Digest();
					}
				}
			} while (!EndOfStack() && depth > 0);
			return buffer.ToArray();
		}

		/// <summary>
		/// ResRefrec
		/// ResolveReferenceRecursively :D
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public Reference[] RRRA<T>(Collection<T> context, int __l = 0, List<Reference> __buffer = null, Token nextToken = null) {
			if (__buffer == null)
				__buffer = new List<Reference>();
			Log.Spam("rrr inbound,\n __level: " + __l);
			Reference r;
			Token t = Peek();
			T key;
			if ((Keywords.IsAlias(t.Str(), Keyword.CURRENT_CONTEXT) && __l == 0)
				|| Keywords.IsAlias(t.Str(), Keyword.PARENT_CONTEXT)
				|| t.Check(ValueType.Identifier) || t.Check(ValueType.Integer)
			) {
				if (nextToken != null) {
					t = nextToken;
					key = t.GetValue<T>();
					Digest();
					r = context.Get(key);
				} else if (t.Value.Equals("len")) {
					r = new Reference(
						new QValue(context.Value.Count, ValueType.Integer),
						null,
						this.stack[0] + ".len",
						Access.PUBLIC,
						true,
						ValueType.Integer
					);
					Digest();
					__buffer.Add(r);
					return __buffer.ToArray();
				} else {
					key = (t.GetValue() is Keyword ? Digest().GetValue<Keyword>().GetKey<T>() : Digest().GetValue<T>());
					r = context.Get(key);
				}
				__buffer.Add(r);
				Log.Spam("found " + (r == null ? " nothing, lol." : r.GetTrueValue()?.Str()));
			} else {
				throw new ParseException("tried to resolve reference with unexpected token '" + t + "'", t);
			}
			if (r == null && __l == 0 && context is Qontext) {
				r = (context as Qontext).LookupOrThrow(key.ToString());
			}
			if (r == null && key != null) {
				throw new ParseException($"Unknown keyword: '{t.Value}'");
			}

			t = Peek();
			if (t.Check(Struqture.MEMBER)) {
				Reference nc = Native.Get(r.GetTrueValue().Type, Peek(1).Str());
				if (nc == null) {
					if (r?.GetTrueValue() is Qontext) {
						Digest(); // <~ into the void!
						return RRRA((Qontext) r.GetTrueValue(), ++__l, __buffer);
					} else if (r?.GetTrueValue() is Array) {
						Digest();
						Token nextT = Peek();
						if (nextT.Value.Equals("len"))
							return RRRA((Array) r.GetTrueValue(), ++__l, __buffer, null);
						if (nextT.Type == ValueType.Identifier) {
							if (context is Qontext globalContext) {
								Reference val = globalContext.Get(nextT.Value.ToString());
								if (val == null)
									throw new ParseException("tried to access a value as indexer which does not exist!", nextT);
								nextToken = Token.Create(
									ValueType.Integer,
									val.GetTrueValue().GetValue().ToString(),
									nextT.line,
									nextT.col
								);
							}
						}
						return RRRA((Array) r.GetTrueValue(), ++__l, __buffer, nextToken);
					}
				} else {
					if (nc != null) {
						Digest();
						Digest();
						__buffer.Add(nc);
						return __buffer.ToArray();
					}
				}
				throw new ParseException("tried to access member of a value that is not a context and thus memberless,\nor tried to call unknown native funqtion.", t);
			} else {
				if (r == null && key != null) {
					Log.Spam("rrr detected undeclared identifier at the end of the member chain,\nreturning floating reference for possible binding...");
					r = new FloatingReference<T>(key, context);
					__buffer.Add(r);
				}
				return __buffer.ToArray();
			}
		}

		public Reference RRR(Qontext context, int item = 0) {
			Reference[] r = RRRA(context);
			return r.Length > item ? r[^(item + 1)] : null;
		}

		public QValue ReadNextValue(Qontext context, ValueType expected = ValueType.Any) {
			Log.Spam("Interpretoken.readNextValue()");
			Token t = Peek();
			QValue result = null;
			if (t.Check(ValueType.Identifier)
				|| (t.Check(ValueType.Keyword) && (t.Check(Keyword.CURRENT_CONTEXT.name) || t.Check(Keyword.PARENT_CONTEXT.name)))
			) {
				Log.Spam("detected possible reference / identifier");
				Reference[] _r = RRRA(context);
				Reference r = _r[^1];
				if (Peek().Check(Struqture.Call[OPEN])) {
					Log.Spam("hey, it's a funqtion call!");
					if (r.GetValueType() == ValueType.Funqtion || r.GetValueType() == ValueType.NativeCall) {
						QValue[] p = Funqtionizer.ParseParameters(context, ReadBody(true));
						result = (r.GetTrueValue() as Funqtion).Execute(p, (_r.Length > 1 ? _r[^2] : null));
					} else {
						throw new ParseException("can not call " + Peek(-1) + ": not a funqtion.");
					}
				} else {
					result = r;
				}
			} else if (t.Check(ValueType.Struqture)) {
				Log.Spam("detected possible structure");
				if (t.Check(Struqture.Funqtion[OPEN])) {
					Log.Spam("...it's a funqtion");
					Funqtion f = Funqtionizer.Parse(context, ReadBody(true));
					if (Peek().Check(Struqture.Call[OPEN])) {
						QValue[] p = Funqtionizer.ParseParameters(context, ReadBody(true));
						result = f.Execute(p);
					} else {
						result = f;
					}
				} else if (t.Check(Struqture.Context[OPEN])) {
					Obqect o = Colleqtionizer.ReadObqect(context, ReadBody(true));
					result = o;
				} else if (t.Check(Struqture.Collection[OPEN])) {
					Array a = Colleqtionizer.ReadArray(context, ReadBody(true));
					result = a;
				} else if (t.Check(Struqture.Call[OPEN])) {
					Segment s = Segmentizer.ParseOne(context, ReadBody());
					result = s.Execute(context);
				}
			} else if (t.Check(ValueType.Primitive)) {
				result = Digest().MakeValue();
			} else if (t.Check(ValueType.Operator)) {
				throw new ParseException("next token was unreadable as value: " + t, t);
			}
			return result;
		}
	}
}
