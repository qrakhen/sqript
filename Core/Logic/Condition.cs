using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    /*internal abstract class Condition : Funqtion
    {
        protected Segment premise;

        public Condition(Context parent, Segment premise = null) : base(parent) {
            this.premise = premise;
        }

        public void setPremise(Segment premise) {
            this.premise = premise;
        }
    }

    internal class IfCondition : Condition
    {
        public Condition elseCondition { get; protected set; }

        public IfCondition(Context parent, Segment premise = null) : base(parent, premise) {

        }

        public void setElse(Condition condition) {
            elseCondition = condition;
        }

        public override Value execute(Value[] parameters = null) {
            if (premise == null) base.execute(null);
            else {
                Value p = premise.execute(parent);
                if (!p.isType(ValueType.BOOLEAN)) throw new ConditionException("expression for if condition has to return a value of type BOOL, got " + p.type.ToString() + " instead.");
                if (p.getValue<bool>()) {
                    base.execute(null);
                } else {
                    elseCondition.execute();
                }
            }
            return null;
        }
    }

    internal class LoopCondition : Funqtion
    {
        public enum LoopType
        {
            None,
            HeaderCondition,
            FooterCondition
        }

        public LoopType loopType { get; protected set; }

        public LoopCondition(Context parent, LoopType loopType, Segment premise) : base(parent, premise) {
            this.loopType = loopType;
        }

        public override Value execute(Value[] parameters = null) {
            Value p = new Value(true, ValueType.BOOLEAN);
            do {
                if (loopType == LoopType.HeaderCondition) p = premise.execute(parent);
                if (!p.isType(ValueType.BOOLEAN)) throw new ConditionException("expression for loop condition has to return a value of type BOOL, got " + p.type.ToString() + " instead.");
                if (p.getValue<bool>()) base.execute(null);
                else break;
                if (loopType == LoopType.FooterCondition) p = premise.execute(parent);
            } while (true);
            return null;
        }
    }*/
}
