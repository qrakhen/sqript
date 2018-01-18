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
            Value r = Value.NULL;
            do {
                Token t = peek();

            } while (!endOfStack());
            result = r;
            return position;
        }



        public static int readValue(Context context, Token[] stack, out Value result, ValueType expected = ValueType.ANY_VALUE) {
            return new Vactory(stack).readNextValue(context, out result, expected);
        }
    }
}
