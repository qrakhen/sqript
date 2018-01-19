using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    internal class Funqtionizer : Interpretoken
    {
        public Funqtionizer(Token[] stack) : base(stack) { }

        public static Funqtion parse(Context context, Token[] stack) {
            return new Funqtionizer(stack).parse(context);
        }

        public Funqtion parse(Context context) {
            Token t = digest();
            if (t.check(Struqture.Funqtion[OPEN])) {
                Funqtion fq = new Funqtion(context);
                do {
                    t = peek();
                    if (t.check(Struqture.Context[OPEN])) break;
                    else if (t.check(ValueType.IDENTIFIER)) {
                        fq.parameters.Add(t.str());
                        digest();
                    } else throw new ParseException("unexpected token found when trying to parse funqtion parameter declaration", t);
                } while (!endOfStack());
                if (endOfStack()) throw new ParseException("unexpected end of stack when trying to parse funqtion parameter declaration", t);
                else {
                    Token[] body = readBody();
                    fq.statements.AddRange(new Statementizer(body).parse(fq));
                    if (peek().check(Struqture.Funqtion[CLOSE])) {
                        return fq;
                    } else if (peek().check(Struqture.Funqtion[DEL])) {
                        throw new FunqtionizerException("funqtions overloads not yet implemented", peek());
                    } else throw new ParseException("unexpected token found when trying to parse funqtion body definition", peek());
                }
            } else throw new ParseException("unexpected funqtion parameter opening, expected '('", t);
        }

        public Value[] parseParameters(Context context) {
            List<Value> parameters = new List<Value>();
            Token t = digest();
            if (t.check(Struqture.Call[OPEN])) {
                t = peek();
                if (t.check(Struqture.Funqtion[CLOSE])) return new Value[0];
                else do {
                        t = digest();
                        if (t.isType(ValueType.ANY_VALUE)) parameters.Add(t.makeValue());
                        else if (t.isType(ValueType.IDENTIFIER)) parameters.Add(context.getOrThrow(t.str()).getReference());
                        else throw new ParseException("unexpected token found when trying to parse funqtion call", t);
                        t = digest();
                        if (t.check(Struqture.Call[DEL])) continue;
                        else if (t.check(Struqture.Call[CLOSE])) break;
                        else throw new ParseException("unexpected token found when trying to parse funqtion call", t);
                    } while (!endOfStack());
                return parameters.ToArray();
            } else throw new ParseException("unexpected funqtion call parameter opening, expected '('", t);
        }

        public static Value[] parseParameters(Context context, Token[] stack) {
            return new Funqtionizer(stack).parseParameters(context);
        }
    }

    internal class FunqtionizerException : Exception
    {
        public FunqtionizerException(string message, Token cause = null) : base(message, cause) { }
    }
}
