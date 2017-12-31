using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public abstract class Condition : Funqtion
    {
        public const string
            CHAR_OPEN = "{",
            CHAR_CLOSE = "}";

        public Context parent { get; protected set; }
        public Expression expression { get; protected set; }
        public Condition elseCondition { get; protected set; }

        public Condition(Context parent) : base(parent) {

        }
    }
}
