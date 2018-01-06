using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public class Conditionizer : Interpretoken
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
                if (keyword.name == Keyword.CONDITION_LOOP) return parse(LoopCondition.LoopType.FooterCondition, context);
                else if (keyword.name == Keyword.CONDITION_IF) return parse(context);
                else throw new ParseException("unexpected keyword when parsing condition: " + keyword.name, t);
            } else if (t.check(CF_BODY_OPEN)) {
                if (keyword.name == Keyword.CONDITION_LOOP) {
                    return parse(LoopCondition.LoopType.FooterCondition, context);                    
                } else if (keyword.name == Keyword.CONDITION_ELSE) {
                    return parse(context);
                } else throw new ParseException("unexpected keyword when parsing condition: " + keyword.name, t);
            } else throw new ParseException("unexpected token when parsing condition", t);
        }

        private IfCondition parse(Context context) {
            Expression expr;
            if (peek().check(CF_EXPR_OPEN)) expr = Expressionizer.parse(context, readBody());
            else expr = null;
            IfCondition c = new IfCondition(context, expr);
            do {
                Token t = peek();
                Keyword k = Keywords.get(t.str());
                if (k != null && k.name == Keyword.CONDITION_ELSE) {
                    digest();
                    k = Keywords.get(peek().str());
                    if (k == null) k = Keywords.get(Keyword.CONDITION_ELSE);
                    else digest();
                    c.setElse(parse(k, context, remaining()));
                } else {
                    if (t.check(CF_BODY_OPEN)) c.statements.AddRange(Statementizer.parse(c, readBody()));
                    else c.statements.AddRange(Statementizer.parse(c, readBody(false, ";")));
                }
            } while (!endOfStack());
            return c;
        }

        private LoopCondition parse(LoopCondition.LoopType loopType, Context context) {
            return null;
        }

        public static Condition parse(Keyword keyword, Context context, Token[] stack) {
            return new Conditionizer(stack).parse(keyword, context);
        }
    }
}
