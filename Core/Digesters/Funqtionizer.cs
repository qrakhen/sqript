using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Funqtionizer : Interpretoken
    {
        public const string
            FQ_DECLARE_OPEN = ":(",
            FQ_CALL_OPEN = ":(",
            FQ_CLOSE = ")",
            FQ_BODY_OPEN = "{",
            FQ_BODY_CLOSE = "}",
            FQ_OVERLOAD_DELIMITER = "",
            FQ_PARAMETER_LIMITER = ",";

        public Funqtionizer(Token[] stack) : base(stack) { }

        public static Funqtion parse(Context context, Token[] stack) {
            return new Funqtionizer(stack).parse(context);
        }

        public Funqtion parse(Context context) {
            Token t = digest();
            if (t.check(FQ_DECLARE_OPEN)) {
                Funqtion fq = new Funqtion(context);
                do {
                    t = peek();
                    if (t.check(FQ_BODY_OPEN)) break;
                    else if (t.check(ValueType.IDENTIFIER)) {
                        fq.parameters.Add(t.str());
                        digest();
                    } else throw new ParseException("unexpected token found when trying to parse funqtion parameter declaration", t);
                } while (!endOfStack());
                if (endOfStack()) throw new ParseException("unexpected end of stack when trying to parse funqtion parameter declaration", t);
                else {
                    Token[] body = readBody();
                    fq.statements.AddRange(new Statementizer(body).parse(fq));
                    if (peek().check(FQ_CLOSE)) {
                        return fq;
                    } else if (peek().check(FQ_OVERLOAD_DELIMITER)) {
                        throw new FunqtionizerException("funqtions overloads not yet implemented", peek());
                    } else throw new ParseException("unexpected token found when trying to parse funqtion body definition", peek());
                }
            } else throw new ParseException("unexpected funqtion parameter opening, expected '('", t);
        }

        public Value[] parseParameters(Context context) {
            List<Value> parameters = new List<Value>();
            Token t = digest();
            if (t.check(FQ_CALL_OPEN)) {
                t = peek();
                if (t.check(FQ_CLOSE)) return new Value[0];
                else do {
                        t = digest();
                        if (t.isType(ValueType.ANY_VALUE)) parameters.Add(t.makeValue());
                        else if (t.isType(ValueType.IDENTIFIER)) parameters.Add(context.getOrThrow(t.str()).getReference());
                        else throw new ParseException("unexpected token found when trying to parse funqtion call", t);
                        t = digest();
                        if (t.check(FQ_PARAMETER_LIMITER)) continue;
                        else if (t.check(FQ_CLOSE)) break;
                        else throw new ParseException("unexpected token found when trying to parse funqtion call", t);
                    } while (!endOfStack());
                return parameters.ToArray();
            } else throw new ParseException("unexpected funqtion call parameter opening, expected '('", t);
        }

        public static Value[] parseParameters(Context context, Token[] stack) {
            return new Funqtionizer(stack).parseParameters(context);
        }
    }

    public class FunqtionizerException : Exception
    {
        public FunqtionizerException(string message, Token cause = null) : base(message, cause) { }
    }
}
