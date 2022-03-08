using System.Collections.Generic;

namespace Qrakhen.Sqript {

	internal class Segmentizer : Interpretoken {

		public Segmentizer(Token[] stack) : base(stack) { }

		public static Segment[] Parse(Qontext context, Token[] stack) {
			return new Segmentizer(stack).Parse(context);
		}

		public static Segment ParseOne(Qontext context, Token[] stack) {
			return new Segmentizer(stack).ParseOne(context);
		}

		public Segment ParseOne(Qontext context) {
			Segment[] segment = Parse(context, true);
			return segment.Length > 0 ? segment[0] : null;
		}

		public Segment[] Parse(Qontext context, bool first = false) {
			Log.Spam("Statementizer.Execute()");
			if (stack.Length == 0)
				return new Segment[0];
			List<Token> buffer = new List<Token>();
			List<Segment> segments = new List<Segment>();
			int level = 0;
			do {
				Token t = Digest();
				if (t.Check(ValueType.Keyword) && t.GetValue<Keyword>().IsType(Keyword.KeywordType.DECLARATION)) {
					if (buffer.Count > 0)
						throw new ParseException("unexpected declaration keyword '" + t.Str() + "' while parsing segment.", t);
					Keyword keyword = t.GetValue<Keyword>();
					if (keyword == Keyword.FUNQTION) {
						t = Peek();
						if (!t.Check(ValueType.Identifier))
							throw new ParseException("expected identifier for declaration of '" + keyword.Name + "', got '" + t.Str() + "' instead.", t);
						string name = Digest().Str();
						Funqtion fq = Funqtionizer.Parse(context, ReadBody(true));
						context.Set(name, new Reference(fq));
					}
					if (Peek().Check(";"))
						Digest();
				} else if (t.Check(ValueType.Keyword) && t.GetValue<Keyword>().IsType(Keyword.KeywordType.CONDITION)) {
					segments.Add(ParseCondition(context, t.GetValue<Keyword>()));
				} else {
					if (t.Check(";") && level == 0) {
						if (first) {
							return new Segment[] { new Segment(buffer.ToArray()) };
						} else {
							segments.Add(new Segment(buffer.ToArray()));
						}

						buffer.Clear();
					} else {
						if (t.Check(Struqture.Context[OPEN])) {
							level++;
						} else if (t.Check(Struqture.Context[CLOSE])) {
							level--;
						}
						buffer.Add(t);
						if (EndOfStack())
							segments.Add(new Segment(buffer.ToArray()));
					}
				}
			} while (!EndOfStack());
			return segments.ToArray();
		}

		protected Segment ParseCondition(Qontext context, Keyword keyword) {
			if (keyword == Keyword.CONDITION_IF) {
				// ToDo Error
				Token[] premise = ReadBody();
				Segment[] body = Parse(context, ReadBody());
				Log.Spam("creating new ifElseSegment");
				IfElseSegment ifElse = new IfElseSegment(body, premise);
				if (Peek().Check(ValueType.Keyword) && Peek().GetValue<Keyword>() == Keyword.CONDITION_ELSE) {
					Digest();
					if (Peek().Check(ValueType.Keyword) && Peek().Check(Keyword.CONDITION_IF)) {
						Digest();
						Log.Spam("appending else if (...) continuation...");
						ifElse.Append((IfElseSegment) ParseCondition(context, keyword));
					} else {
						Log.Spam("appending premise-less else continuation...");
						ifElse.Append(new IfElseSegment(Parse(context, ReadBody()), new Token[0]));
					}
				}
				return ifElse;
			} else if (keyword == Keyword.CONDITION_LOOP) {
				if (Peek().Check(ValueType.Struqture) && Peek().Check(Struqture.Call[OPEN])) {
					Token[] premise = ReadBody();
					Segment[] body = Parse(context, ReadBody());
					LoopSegment loop = new LoopSegment(body, LoopSegment.HEAD, premise);
					return loop;
				} else if (Peek().Check(ValueType.Struqture) && Peek().Check(Struqture.Context[OPEN])) {
					Segment[] body = Parse(context, ReadBody());
					Token[] premise = ReadBody();
					LoopSegment loop = new LoopSegment(body, LoopSegment.FOOT, premise);
					return loop;
				} else {
					new OperationException("loop condition expected '(' or '{' to start premise or body - got '" + Peek().Str() + "' instead.");
				}
			}
			throw new OperationException("wanted to parse condition, but could not. debug me!!");
		}
	}
}
