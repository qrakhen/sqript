using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    internal class Segment : Interpretoken
    {
        protected Node head;

        public Segment(Token[] stack) : base(stack) {

        }

        /**
         *  *~ a <~ 5 + (8 / 2);
         *  
         *     { <- head node
         *     a <~ {
         *          5 + {
         *              (5 / 2); <- another segment with its own head node
         *              }
         *          }
         *     }
         * */
        public Value execute(Context context) {
            return build(context).execute();
        }

        protected virtual Node build(Context context, Node pre = null) {
            do {
                if (pre == null) pre = new Node();
                int p = position;
                Value v = Vactory.readNextValue(context, stack, out p);
            } while (!endOfStack());
            return null;
        }

        protected class Node 
        {
            public object left { get; set; }
            public object right { get; set; }
            public Operator op { get; set; }

            public Node(object left = null, object right = null, Operator op = null) {
                this.left = left;
                this.right = right;
                this.op = op;
            }

            public Value execute() {
                if (left != null) {
                    if (op != null && right != null) {
                        Value _left = (Value)left;
                        Value _right;
                        if (right is Node) _right = (right as Node).execute();
                        else _right = (Value)right;
                        return op.execute(_left, _right);
                    } else {
                        return (Value)left;
                    }
                } else {
                    throw new OperationException("tried to execute empty SegmentNode");
                }
            }

            public bool ready() {
                return (left != null && right != null && op != null);
            }

            public bool put(object value) {
                if (left == null) left = value;
                else if (right == null) right = value;
                else return false;
                return true;
            }

            public override string ToString() {
                string r = "{\n";
                r += "    " + left?.ToString() + " " + op?.symbol;
                string[] s = right?.ToString().Split(new char[] { '\n' });
                r += " " + s[0] + " ";
                for (int i = 1; i < s.Length; i++) r += "    " + s[i];
                return r += "\n}";
            }
        }
    }    

    internal class TypeDefinition : Segment
    {
        public TypeDefinition(Token[] stack) : base(stack) {

        }
    }

    internal class _Statement : Segment
    {
        public _Statement(Token[] stack) : base(stack) {

        }
    }

    internal class _Expression : Segment
    {
        protected Segment left, right;
        protected Operator op;

        public _Expression(Token[] stack) : base(stack) {

        }
    }
}
