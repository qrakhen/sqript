using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public abstract class Condition : Funqtion
    {
        public Expression premise { get; protected set; }

        public Condition(Context parent, Expression premise = null) : base(parent) {
            this.premise = premise;
        }
    }

    public class IfCondition : Condition
    {
        public Condition elseCondition { get; protected set; }

        public IfCondition(Context parent, Expression premise = null) : base(parent, premise) {

        }

        public void setElse(Condition condition) {
            elseCondition = condition;
        }

        public override Value execute(Value[] parameters = null) {
            Value p = premise.execute();
            if (!p.isType(ValueType.BOOLEAN)) throw new ConditionException("expression for if condition has to return a value of type BOOL, got " + p.type.ToString() + " instead.");
            if (p.getValue<bool>()) {
                base.execute(null);
            } else {
                elseCondition.execute();
            }
            return null;
        }
    }

    public class LoopCondition : Condition
    {
        public enum LoopType
        {
            None,
            HeaderCondition,
            FooterCondition
        }

        public LoopType loopType { get; protected set; }

        public LoopCondition(Context parent, LoopType loopType, Expression premise = null) : base(parent, premise) {
            this.loopType = loopType;
        }

        public override Value execute(Value[] parameters = null) {
            Value p = new Value(true, ValueType.BOOLEAN);
            do {
                if (loopType == LoopType.HeaderCondition) p = premise.execute();
                if (!p.isType(ValueType.BOOLEAN)) throw new ConditionException("expression for loop condition has to return a value of type BOOL, got " + p.type.ToString() + " instead.");
                if (p.getValue<bool>()) base.execute(null);
                else break;
                if (loopType == LoopType.FooterCondition) p = premise.execute();
            } while (true);
            return null;
        }
    }
}
