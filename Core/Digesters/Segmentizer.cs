using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    internal class Segmentizer : Interpretoken
    {
        public Segmentizer(Token[] stack) : base(stack) { }

        public static Segment[] parse(Context context, Token[] stack) {
            return new Segmentizer(stack).parse(context);
        }

        /**
         *  MAKE SEGMENTIZER -> SPLITS SEGMENTS BY ';' AND DETECTS CONDITION-, CLASS-, FUNCTION-, (etc...) -DEFINTIONS 
         * 
         * */

        public Segment[] parse(Context context) {
            Log.spam("Statementizer.execute()");
            if (stack.Length == 0) return new Segment[0];
            List<Token> buffer = new List<Token>();
            List<Segment> segments = new List<Segment>();
            int level = 0;
            do {
                Token t = digest();
                if (t.check(ValueType.KEYWORD) && t.getValue<Keyword>().isType(Keyword.KeywordType.DECLARATION)) {
                    if (buffer.Count > 0) throw new ParseException("unexpected declaration keyword '" + t.str() + "' while parsing segment.", t);
                    Keyword k = t.getValue<Keyword>();
                    if (k.name == Keyword.FUNQTION.name) {
                        t = peek();
                        if (!t.check(ValueType.IDENTIFIER)) throw new ParseException("expected identifier for declaration of '" + k.name + "', got '" + t.str() + "' instead.", t);
                        string name = digest().str();
                        Funqtion fq = Funqtionizer.parse(context, readBody(true));
                        context.set(name, new Reference(fq));
                    } else if (k.isType(Keyword.KeywordType.CONDITION)) {
                        Segment premise = parse(context, readBody())[0];
                        Value result = premise.execute(context);
                        if (result.type == ValueType.BOOLEAN) {
                            if (result.getValue<bool>() == true) {
                                segments.AddRange(parse(context, readBody()));
                            } else {
                                readBody(); //ignore
                                if (peek().check(Keyword.CONDITION_ELSE.name)) {
                                    digest();
                                }
                            }
                        } else {
                            throw new OperationException("condition premise has to return a boolean type value, got '" + result.type + "' instead.");
                        }
                    }
                    if (peek().check(";")) digest();
                } else {
                    if (t.check(";") && level == 0) {
                        segments.Add(new Segment(buffer.ToArray()));
                        buffer.Clear();
                    } else {
                        if (t.check(Context.CHAR_OPEN)) level++;
                        else if (t.check(Context.CHAR_CLOSE)) level--;
                        buffer.Add(t);
                        if (endOfStack()) segments.Add(new Segment(buffer.ToArray()));
                    }
                }
            } while (!endOfStack());
            return segments.ToArray();
        }
    }
}
