using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript.Core.Digesters
{
    internal class Vactory : Interpretoken
    {
        public Vactory(Token[] stack) : base(stack) {

        }

        public int readNextValue(Context context, out Value result, ValueType expected = ValueType.ANY_VALUE) {
            do {
                Token t = peek();
                if ((t.check(ValueType.IDENTIFIER)) || 
                        (t.check(ValueType.KEYWORD) && 
                        (t.check(Keyword.CURRENT_CONTEXT) || 
                        t.check(Keyword.PARENT_CONTEXT)))) {
                    Reference r = rrr(context);
                    if (peek().check(Funqtionizer.FQ_CALL_OPEN)) {
                        Value[] p = Funqtionizer.parseParameters(context, readBody(true));
                        if (r.getValueType() == ValueType.FUNQTION) {
                            result = (r.getReference() as Funqtion).execute(p);
                            return position;
                        } else throw new ParseException("can not call " + peek(-1) + ": not a funqtion.");
                    } else {
                        result = r;
                        return position;
                    }
                } else if (t.check(ValueType.STRUCTURE)) {
                    if (t.check(Funqtionizer.FQ_DECLARE_OPEN)) {
                        Funqtion f = Funqtionizer.parse(context, readBody(true));
                        if (peek().check(Funqtionizer.FQ_CALL_OPEN)) {
                            Value[] p = Funqtionizer.parseParameters(context, readBody(true));
                            result = f.execute(p);
                            return position;
                        } else {
                            result = f;
                            return position;
                        }
                    } else if (t.check(Obqect.CHAR_OPEN)) {
                        //Obqect o = readBody(true);
                    } else if (t.check(Array.CHAR_OPEN)) {
                        //Array a = readBody(true);
                    } else if (t.check("(")) {
                        Expression e = Expressionizer.parse(context, readBody());
                    }
                } else if (t.check(ValueType.OPERATOR)) {
                    throw new ParseException("next token was unreadable as value: " + t, t);
                } else if (t.check(ValueType.ANY_VALUE)) {
                    result = digest().makeValue();
                    return position;
                }
            } while (!endOfStack());
            result = Value.NULL;
            return position;
        }



        public static int readValue(Context context, Token[] stack, out Value result, ValueType expected = ValueType.ANY_VALUE) {
            return new Vactory(stack).readNextValue(context, out result, expected);
        }
    }
}
