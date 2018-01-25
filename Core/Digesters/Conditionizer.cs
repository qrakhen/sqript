using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    /*internal class Conditionizer : Interpretoken
    {
        public const string
            CF_EXPR_OPEN = "(",
            CF_EXPR_CLOSE = ")",
            CF_BODY_OPEN = "{",
            CF_BODY_CLOSE = "}";

        public Conditionizer(Token[] stack) : base(stack) {

        }

        public Condition parse(Keyword keyword, Context context) {
            Token t = peek();
            if (t.check(CF_EXPR_OPEN)) {
                if (keyword.name == Keyword.CONDITION_LOOP.name) return parseLoop(context);
                else if (keyword.name == Keyword.CONDITION_IF.name) return parseIf(context);
                else throw new ParseException("unexpected keyword when parsing condition: " + keyword.name, t);
            } else if (t.check(CF_BODY_OPEN)) {
                if (keyword.name == Keyword.CONDITION_LOOP.name) {
                    return parseLoop(context);                    
                } else if (keyword.name == Keyword.CONDITION_ELSE.name) {
                    return parseIf(context);
                } else throw new ParseException("unexpected keyword when parsing condition: " + keyword.name, t);
            } else throw new ParseException("unexpected token when parsing condition", t);
        }

        private IfCondition parseIf(Context context) {
            Segment premise;
            if (peek().check(CF_EXPR_OPEN)) premise = Segmentizer.parse(context, readBody())[0];
            else premise = null;
            IfCondition c = new IfCondition(context, premise);
            do {
                Token t = peek();
                Keyword k = Keywords.get(t.str());
                if (k != null && k.name == Keyword.CONDITION_ELSE.name) {
                    digest();
                    k = Keywords.get(peek().str());
                    if (k == null) k = Keywords.get(Keyword.CONDITION_ELSE.name);
                    else digest();
                    c.setElse(parse(k, context, remaining()));
                } else {
                    if (t.check(CF_BODY_OPEN)) c.segments.AddRange(Segmentizer.parse(c, readBody()));
                    else c.segments.AddRange(Segmentizer.parse(c, readBody(false, ";")));
                }
            } while (!endOfStack());
            return c;
        }

        private LoopCondition parseLoop(Context context) {
            Segment premise;
            if (peek().check(CF_EXPR_OPEN)) premise = Segmentizer.parse(context, readBody())[0];
            else premise = null;
            var loopType = (premise == null ? LoopCondition.LoopType.FooterCondition : LoopCondition.LoopType.HeaderCondition);
            LoopCondition c = new LoopCondition(context, loopType, premise);
            do {
                Token t = peek();
                Keyword k = Keywords.get(t.str());
                if (k != null && k.name == Keyword.CONDITION_LOOP.name && premise == null) {
                    digest();
                    premise = Segmentizer.parse(context, readBody())[0];
                    c.setPremise(premise);
                } else {
                    if (t.check(CF_BODY_OPEN)) c.segments.AddRange(Segmentizer.parse(c, readBody()));
                    else c.segments.AddRange(Segmentizer.parse(c, readBody(false, ";")));
                }
            } while (!endOfStack());
            return c;
        }

        public static Condition parse(Keyword keyword, Context context, Token[] stack) {
            return new Conditionizer(stack).parse(keyword, context);
        }
    }*/
}
