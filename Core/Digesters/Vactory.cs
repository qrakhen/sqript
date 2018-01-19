using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal class Vactory : Interpretoken
    {
        public Vactory(Token[] stack) : base(stack) {

        }

        public int readNextValue(Context context, out Value result, ValueType expected = ValueType.ANY_VALUE) {
            Log.spam("Vactory.readNextValue()");
            Token t = peek();
            if ((t.check(ValueType.IDENTIFIER)) || 
                    (t.check(ValueType.KEYWORD) && 
                    (t.check(Keyword.CURRENT_CONTEXT) || 
                    t.check(Keyword.PARENT_CONTEXT)))) {
                Log.spam("detected possible reference / identifier");
                Reference r = rrr(context);
                if (peek().check(Struqture.Call[OPEN])) {
                    Log.spam("hey, it's a funqtion call!");
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
                Log.spam("detected possible structure");
                if (t.check(Struqture.Funqtion[OPEN])) {
                    Log.spam("it's a funqtion");
                    Funqtion f = Funqtionizer.parse(context, readBody(true));
                    if (peek().check(Struqture.Call[OPEN])) {
                        Value[] p = Funqtionizer.parseParameters(context, readBody(true));
                        result = f.execute(p);
                        return position;
                    } else {
                        result = f;
                        return position;
                    }
                } else if (t.check(Obqect.CHAR_OPEN)) {
                    Obqect o = Obqectizer.parse(context, readBody(true));
                    result = o;
                    return position;
                } else if (t.check(Array.CHAR_OPEN)) {
                    // Array a = readBody(true);
                } else if (t.check("(")) {
                    Expression e = Expressionizer.parse(context, readBody());
                }
            } else if (t.check(ValueType.OPERATOR)) {
                throw new ParseException("next token was unreadable as value: " + t, t);
            } else if (t.check(ValueType.PRIMITIVE)) {
                result = digest().makeValue();
                return position;
            }
            result = Value.NULL;
            return position;
        }



        public static int readNextValue(Context context, Token[] stack, out Value result, ValueType expected = ValueType.ANY_VALUE) {
            return new Vactory(stack).readNextValue(context, out result, expected);
        }
    }
}
