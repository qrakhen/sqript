﻿using System.Collections.Generic;

namespace Qrakhen.Sqript {
	internal class Segment : Interpretoken {
		public bool returning { get; protected set; }
		protected Node head;

		public SegmentType type { get; protected set; }
		public enum SegmentType {
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
		 *	 { <- head node
		 *	 a <~ {
		 *		  5 + {
		 *			  (5 / 2); <- another segment with its own head node
		 *			  }
		 *		  }
		 *	 }
		 **/
		public virtual Value execute(Qontext context) {
			Log.spam("segment.execute()");
			reset();
			returning = false;
			head = new Node();
			Node tail = build(context, new Node());
			Log.spam("executing node:\n" + tail);
			Value r = Value.Null;
			if(tail.empty() && !head.empty()) {
				r = head.execute();
			} else {
				r = tail.execute();
				if(!head.empty()) {
					head.right = r;
					head.execute();
				}
			}
			if(!head.empty() && head.left is FloatingReference<string>)
				(head.left as FloatingReference<string>).bind();
			else if(!head.empty() && head.left is FloatingReference<int>)
				(head.left as FloatingReference<int>).bind();
			return r;
		}

		protected virtual Node build(Qontext context, Node node = null) {
			int step = 0;
			do {
				if(node == null)
					node = new Node();
				Token t = peek();
				if(t.check(ValueType.Keyword)) {
					if(t.check(Keyword.REFERENCE)) {
						if(node.left != null)
							throw new ParseException("can not declare a new reference here ._.", t);
						type = SegmentType.REFERENCE;
						digest();
						t = peek();
						if(t.check(ValueType.Identifier)) {
							head.left = new FloatingReference<string>(digest().str(), context);
						} else
							throw new ParseException("expected identifier after new reference keyword, got '" + t + "' instead", t);
					} else if(t.check(Keyword.CURRENT_CONTEXT) || t.check(Keyword.PARENT_CONTEXT)) {
						if(head.empty()) {
							head = new Node(rrr(context), null, null);
						} else {
							node.put(rrr(context));
						}
					} else if(t.check(Keyword.RETURN)) {
						digest();
						returning = true;
					} else
						throw new Exception("check me");
				} else if(t.check(ValueType.Operator)) {
					Operator op = digest().getValue<Operator>();
					// ToDo: Operator ==, < und >
					if(op.symbol == Operator.ASSIGN_VALUE || op.symbol == Operator.ASSIGN_REFERENCE) {
						if(step == 0)
							returning = true;
						else if(head.empty() && step == 1) {
							head.left = node.left;
							node.left = null;
						}
					}
					if(node.ready()) {
						Log.spam("comparing operators to chain nodes");
						if(op.compare(node.op) > 0)
							node.right = build(context, new Node(node.right, null, op));
						else
							node = build(context, new Node(node, null, op));
					} else if(head.op == null && (
						op.symbol == Operator.ASSIGN_VALUE ||
						op.symbol == Operator.ASSIGN_REFERENCE)) {
						head.op = op;
					} else if(node.left == null || (node.op != null && node.right == null)) {
						if(op.symbol == Operator.CALCULATE_ADD)
							continue;
						else if(op.symbol == Operator.CALCULATE_SUBTRACT) {
							var neg = new Segment(new Token[] {
								Token.create(ValueType.Number, "-1"),
								Token.create(ValueType.Operator, Operator.CALCULATE_MULTIPLY),
								digest() });
							Log.spam("created negating segment: " + neg.ToString());
							if(node.left == null)
								node.left = neg.execute(context);
							else
								node.right = neg.execute(context);
						} else if(op.symbol == Operator.CONDITION_BIGGER
							|| op.symbol == Operator.CONDITION_BIGGER_EQUAL
							|| op.symbol == Operator.CONDITION_SMALLER
							|| op.symbol == Operator.CONDITION_SMALLER_EQUAL
							|| op.symbol == Operator.CONDITION_EQUALS
						) {
							var tempContext = context;
							if(this.stack[0].isType(ValueType.Identifier)) {
								node.left = tempContext.get(this.stack[0].value.ToString());
							} else {
								node.left = this.stack[0];
							}
							if(this.stack[2].isType(ValueType.Identifier)) {
								node.right = tempContext.get(this.stack[2].value.ToString());
							} else {
								node.right = this.stack[2];
							}
							node.op = op;
							return node;
						} else {
							throw new ParseException("unexpected operator " + op.symbol + " at start of expression", t);
						}
					} else if(node.left != null) {
						node.op = op;
					} else
						throw new ParseException("sorry, manipulation operators (operators without left-hand value) are not yet implemented. :/", t);
				} else if(t.check(ValueType.Struqture, "(")) {
					if(node.ready())
						throw new ParseException("can not open new segment without prior operator.", t);
					Segment sub = new Segment(readBody());
					node.put(sub.execute(context));
				} else {
					if(t.check(";") || endOfStack()) {
						Log.spam("end of node chain reached: ';'");
						digest();
						break;
					}
					if(node.ready())
						throw new ParseException("can not add another value to finished new segment node without prior operator.", t);
					Value v = readNextValue(context);
					node.put(v);
				}
				step++;
			} while(!endOfStack());
			return node;
		}

		public override string ToString() {
			string r = "";
			foreach(Token t in stack) {
				r += " " + t.str();
			}
			if(r.Length > 0)
				r = r.Substring(1) + ";";
			return r;
		}

		protected class Node {
			public object left { get; set; }
			public object right { get; set; }
			public Operator op { get; set; }

			public Node(object left = null, object right = null, Operator op = null) {
				this.left = left;
				this.right = right;
				this.op = op;
			}

			public Value execute() {
				if(left != null) {
					if(op != null && right != null) {
						Value _left;
						Value _right;
						if(right is Node)
							_right = (right as Node).execute();
						else
							_right = (Value) right;
						if(left is Node)
							_left = (left as Node).execute();
						else
							_left = (Value) left;
						return op.execute(_left, _right);
					} else {
						return (Value) left;
					}
				} else {
					return Value.Null;
				}
			}

			public bool isReturner() {
				return (left == null && right != null &&
					(op.symbol == Operator.ASSIGN_VALUE || op.symbol == Operator.ASSIGN_REFERENCE));
			}

			public bool ready() {
				return (left != null && right != null && op != null);
			}

			public bool empty() {
				return (left == null && right == null && op == null);
			}

			public bool put(object value) {
				if(left == null)
					left = value;
				else if(right == null)
					right = value;
				else
					return false;
				return true;
			}

			public override string ToString() {
				if(empty())
					return "{ void }";
				return "{ " + (left ?? "null") + " " + (op == null ? "NaO" : op.symbol) + " " + (right ?? "null") + " }";
			}
		}
	}

	internal class LoopSegment : Segment {
		public const int HEAD = 0x1, FOOT = 0x2;

		public Segment[] body { get; protected set; }
		public int loopType { get; protected set; }

		public LoopSegment(Segment[] body, int loopType, Token[] stack) : base(stack) {
			this.body = body;
			this.loopType = loopType;
		}

		public override Value execute(Qontext context) {
			Value p = new Value(true, ValueType.Boolean);
			do {
				if(loopType == HEAD) {
					p = base.execute(context);
					while(p.isType(ValueType.Reference)) {
						p = (Value) p.value;
					}
				}
				if(!p.isType(ValueType.Boolean))
					throw new ConditionException("expression for loop condition has to return a value of type BOOL, got " + p.type.ToString() + " instead.");
				if(p.getValue<bool>()) {
					Funqtion fq = new Funqtion(context, new List<Segment>(body));
					Value v = fq.execute();
					if(fq.returning) {
						this.returning = true;
						return v;
					}
				} else {
					break;
				}
				if(loopType == FOOT) {
					p = base.execute(context);
					while(p.isType(ValueType.Reference)) {
						p = (Value) p.value;
					}
				}
			} while(true);
			return null;
		}
	}

	internal class IfElseSegment : Segment {
		public Segment[] body { get; protected set; }
		public Segment next { get; protected set; }

		public IfElseSegment(Segment[] body, Token[] stack) : base(stack, SegmentType.CONDITION) {
			this.body = body;
		}

		public void append(IfElseSegment next) {
			this.next = next;
		}

		public override Value execute(Qontext context) {
			Value r = stack.Length == 0 ? Value.True : base.execute(context);
			while(r.isType(ValueType.Reference)) {
				r = (Value) r.value;
			}
			if(!r.isType(ValueType.Boolean))
				throw new ConditionException("expression for loop condition has to return a value of type Boolean, got " + r.type.ToString() + " instead.");
			else if(r.getValue<bool>()) {
				Funqtion xfq = new Funqtion(context, new List<Segment>(body));
				Value v = xfq.execute();
				if(xfq.returning)
					this.returning = true;
				return v;
			} else if(next != null) {
				next.execute(context);
			}
			return null;
		}
	}
}
