using System.Collections.Generic;

namespace Qrakhen.Sqript
{
    internal class Segmentizer : Interpretoken
    {
        public Segmentizer(Token[] stack) : base(stack) { }

        public static Segment[] parse(Qontext context, Token[] stack) {
            return new Segmentizer(stack).parse(context);
        }

        public static Segment parseOne(Qontext context, Token[] stack) {
            return new Segmentizer(stack).parseOne(context);
        }

        public Segment parseOne(Qontext context) {
            return parse(context, true)[0];
        }

        public Segment[] parse(Qontext context, bool first = false) {
            Log.spam("Statementizer.execute()");
            if (stack.Length == 0) return new Segment[0];
            List<Token> buffer = new List<Token>();
            List<Segment> segments = new List<Segment>();
            int level = 0;
            do {
                Token t = digest();
                if (t.check(ValueType.Keyword) && t.getValue<Keyword>().isType(Keyword.KeywordType.DECLARATION)) {
                    if (buffer.Count > 0) throw new ParseException("unexpected declaration keyword '" + t.str() + "' while parsing segment.", t);
                    Keyword keyword = t.getValue<Keyword>();
                    if (keyword == Keyword.FUNQTION) {
                        t = peek();
                        if (!t.check(ValueType.Identifier)) throw new ParseException("expected identifier for declaration of '" + keyword.name + "', got '" + t.str() + "' instead.", t);
                        string name = digest().str();
                        Funqtion fq = Funqtionizer.parse(context, readBody(true));
                        context.set(name, new Reference(fq));
                    }
                    if (peek().check(";")) digest();
                } else if (t.check(ValueType.Keyword) && t.getValue<Keyword>().isType(Keyword.KeywordType.CONDITION)) {
                    segments.Add(parseCondition(context, t.getValue<Keyword>()));
                } else {
                    if (t.check(";") && level == 0) {
                        if (first) return new Segment[] { new Segment(buffer.ToArray()) };
                        else segments.Add(new Segment(buffer.ToArray()));
                        buffer.Clear();
                    } else {
                        if (t.check(Struqture.Context[OPEN])) level++;
                        else if (t.check(Struqture.Context[CLOSE])) level--;
                        buffer.Add(t);
                        if (endOfStack()) segments.Add(new Segment(buffer.ToArray()));
                    }
                }
            } while (!endOfStack());
            return segments.ToArray();
        }

        protected Segment parseCondition(Qontext context, Keyword keyword) {
            if (keyword == Keyword.CONDITION_IF) {
                Token[] premise = readBody();
                Segment[] body = parse(context, readBody());
                Log.spam("creating new ifElseSegment");
                IfElseSegment ifElse = new IfElseSegment(body, premise);
                if (peek().check(ValueType.Keyword) && peek().getValue<Keyword>() == Keyword.CONDITION_ELSE) {
                    digest();
                    if (peek().check(ValueType.Keyword) && peek().check(Keyword.CONDITION_IF)) {
                        digest();
                        Log.spam("appending else if (...) continuation...");
                        ifElse.append((IfElseSegment) parseCondition(context, keyword));
                    } else {
                        Log.spam("appending premise-less else continuation...");
                        ifElse.append(new IfElseSegment(parse(context, readBody()), new Token[0]));
                    }
                }
                return ifElse;
            } else if (keyword == Keyword.CONDITION_LOOP) {
                if (peek().check(ValueType.Struqture) && peek().check(Struqture.Call[OPEN])) {
                    Token[] premise = readBody();
                    Segment[] body = parse(context, readBody());
                    LoopSegment loop = new LoopSegment(body, LoopSegment.HEAD, premise);
                    return loop;
                } else if (peek().check(ValueType.Struqture) && peek().check(Struqture.Context[OPEN])) {
                    Segment[] body = parse(context, readBody());
                    Token[] premise = readBody();
                    LoopSegment loop = new LoopSegment(body, LoopSegment.FOOT, premise);
                    return loop;
                } else new OperationException("loop condition expected '(' or '{' to start premise or body - got '" + peek().str() + "' instead.");
            }
            throw new OperationException("wanted to parse condition, but could not. debug me!!");
        }
    }
}
