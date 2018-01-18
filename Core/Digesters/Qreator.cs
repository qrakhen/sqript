using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript.Core.Digesters
{
    internal class Qreator : Interpretoken
    {
        public Qreator(Token[] stack) : base(stack) {

        }

        public Value readValue(Context context, ValueType expected = ValueType.ANY_VALUE) {
            Value r = Value.NULL;
            do {
                Token t = peek();

            } while (!endOfStack());
            return r;
        }

        public static Value readValue(Context context, Token[] stack, ValueType expected = ValueType.ANY_VALUE) {
            return new Qreator(stack).readValue(context, expected);
        }
    }
}
