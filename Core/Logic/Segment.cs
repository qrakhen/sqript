using System.Collections.Generic;

namespace Qrakhen.Sqript
{
    internal class Segment : Interpretoken
    {
        protected Node head;

        public SegmentType type { get; protected set; }
        public enum SegmentType
        {
            UNDEFINED = 0x00,
            EXPRESSION = 0x01,
            REFERENCE = 0x02,
            CONDITION = 0x04,
            FUNQTION = 0x08,
            CLASS = 0x10
        }

        public Segment(Token[] stack, SegmentType type = SegmentType.UNDEFINED) : base(stack) {
            this.type = type;
        }

        /************************************
         *  *~ a <~ 5 + (8 / 2);
         *  
         *     { <- head node
         *     a <~ {
         *          5 + {
         *              (5 / 2); <- another segment with its own head node
         *              }
         *          }
         *     }
         **/
        public virtual Value execute(Qontext context) {
            Log.spam("segment.execute()");
            reset();
            head = new Node();
            Node end = build(context);
            Value r = Value.NULL;
            if (end.empty() && !head.empty()) {
                r = head.execute();
            } else {
                r = end.execute();
                if (!head.empty()) {
                    head.right = r;
                    head.execute();
                }
            }
            if (!head.empty() && head.left is FloatingReference) (head.left as FloatingReference).bind();
            return r;
        }

        protected virtual Node build(Qontext context, Node node = null) {
            int step = 0;
            do {
                if (node == null) node = new Node();
                Log.spam("now building node: " + node);
                Token t = peek();
                if (t.check(ValueType.KEYWORD)) {
                    if (t.check(Keyword.REFERENCE)) {
                        if (node.left != null) throw new ParseException("can not declare a new reference here ._.", t);
                        type = SegmentType.REFERENCE;
                        digest();
                        t = peek();
                        if (t.check(ValueType.IDENTIFIER)) {
                            head.left = new FloatingReference(digest().str(), context);
                        } else throw new ParseException("expected identifier after new reference keyword, got '" + t + "' instead", t);
                    } else if (t.check(Keyword.CURRENT_CONTEXT) || t.check(Keyword.PARENT_CONTEXT)) {
                        if (head.empty()) {
                            head = new Node(rrr(context), null, null);
                        } else {
                            node.put(rrr(context));
                        }
                    } else throw new Exception("check me");
                } else if (t.check(ValueType.OPERATOR)) {
                    Operator op = digest().getValue<Operator>();
                    if (op.symbol == Operator.ASSIGN_VALUE || op.symbol == Operator.ASSIGN_REFERENCE) {
                        if (head.empty() && step == 1) {
                            head.left = node.left;
                        }
                    }
                    if (node.ready()) {
                        // explanation: move right node side to the new node and make that the right side of the current node
                        // let's call it node stack rotation or something, sounds pretty cool
                        Node next = new Node(node, null, op);
                        node = build(context, next);
                    } else if (node.left != null) {
                        node.op = op;
                    } else if (head.op == null) {
                        head.op = op;
                    } else throw new ParseException("sorry, manipulation operators (operators without left-hand value) are not yet implemented. :/", t);
                } else if (t.check(ValueType.STRUCTURE, "(")) {
                    if (node.ready()) throw new ParseException("can not open new segment without prior operator.", t);
                    Segment sub = new Segment(readBody());
                    node.put(sub.execute(context));
                } else {
                    if (t.check(";") || endOfStack()) {
                        Log.spam("end of node chain reached: ';'");
                        digest();
                        break;
                    }
                    if (node.ready()) throw new ParseException("can not add another value to finished new segment node without prior operator.", t);
                    Value v = readNextValue(context);
                    node.put(v);
                }
                step++;
            } while (!endOfStack());
            return node;
        }

        public override string ToString() {
            string r = "";
            foreach (Token t in stack) {
                r += " " + t.str();
            }
            if (r.Length > 0) r = r.Substring(1) + ";";
            return r;
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
                        Value _left;
                        Value _right;
                        if (right is Node) _right = (right as Node).execute();
                        else _right = (Value)right;
                        if (left is Node) _left = (left as Node).execute();
                        else _left = (Value)left;
                        return op.execute(_left, _right);
                    } else {
                        return (Value)left;
                    }
                } else {
                    return Value.NULL;
                }
            }

            public bool ready() {
                return (left != null && right != null && op != null);
            }

            public bool empty() {
                return (left == null && right == null && op == null);
            }

            public bool put(object value) {
                if (left == null) left = value;
                else if (right == null) right = value;
                else return false;
                return true;
            }

            public override string ToString() {
                if (empty()) return "{ }";
                string r = "{\n";
                r += "   " + (right is Node ? left?.ToString() : right?.ToString()) + " " + op?.symbol;
                if (ready()) {
                    string[] s = (right is Node ? right?.ToString() : left?.ToString()).Split(new char[] { '\n' });
                    r += " " + s[0] + " ";
                    for (int i = 1; i < s.Length; i++) r += "\n    " + s[i];
                }
                return r += "\n}";
            }
        }
    }

    internal class LoopSegment : Segment
    {
        public const int HEAD = 0x1, FOOT = 0x2;

        public Segment[] body { get; protected set; }
        public int loopType { get; protected set; }

        public LoopSegment(Segment[] body, int loopType, Token[] stack) : base(stack) {
            this.body = body;
            this.loopType = loopType;
        }

        public override Value execute(Qontext context) {
            Value p = new Value(true, ValueType.BOOLEAN);
            do {
                if (loopType == HEAD) p = base.execute(context);
                if (!p.isType(ValueType.BOOLEAN)) throw new ConditionException("expression for loop condition has to return a value of type BOOL, got " + p.type.ToString() + " instead.");
                if (p.getValue<bool>()) {
                    Funqtion fq = new Funqtion(context, new List<Segment>(body));
                    fq.execute();
                } else break;
                if (loopType == FOOT) p = base.execute(context);
            } while (true);
            return null;
        }
    }

    internal class IfElseSegment : Segment
    {
        public Segment[] body { get; protected set; }
        public Segment next { get; protected set; }

        public IfElseSegment(Segment[] body, Token[] stack) : base(stack) {
            this.body = body;
        }

        public void append(IfElseSegment next) {
            this.next = next;
        }

        public override Value execute(Qontext context) {
            Value r = stack.Length == 0 ? Value.TRUE : base.execute(context);
            if (!r.isType(ValueType.BOOLEAN)) throw new ConditionException("expression for loop condition has to return a value of type BOOLEAN, got " + r.type.ToString() + " instead.");
            else if (r.getValue<bool>()) {
                Funqtion xfq = new Funqtion(context, new List<Segment>(body));
                xfq.execute();
            } else if (next != null) {
                next.execute(context);
            }
            return null;
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
