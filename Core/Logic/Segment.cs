using System.Collections.Generic;

namespace Qrakhen.Sqript
{

	internal class Segment : Interpretoken
	{

		public bool Returning { get; protected set; }
		protected Node Head;

		public SegmentType Type { get; protected set; }
		public enum SegmentType {
			UNDEFINED = 0x00,
			EXPRESSION = 0x01,
			REFERENCE = 0x02,
			CONDITION = 0x04,
			FUNQTION = 0x08,
			CLASS = 0x10
		}


		public Segment(Token[] stack, SegmentType type = SegmentType.UNDEFINED) : base(stack) {
			this.Type = type;
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
		public virtual QValue Execute(Qontext context) {
			Log.Spam("segment.Execute()");
			Reset();
			Returning = false;
			Head = new Node();
			Node tail = Build(context, new Node());
			Log.Spam("executing node:\n" + tail);
			QValue r;
			if (tail.Empty() && !Head.Empty()) {
				r = Head.Execute();
			} else {
				r = tail.Execute();
				if (!Head.Empty()) {
					Head.Right = r;
					Head.Execute();
				}
			}
			if (!Head.Empty() && Head.Left is FloatingReference<string>)
				(Head.Left as FloatingReference<string>).Bind();
			else if (!Head.Empty() && Head.Left is FloatingReference<int>)
				(Head.Left as FloatingReference<int>).Bind();
			return r;
		}

		protected virtual Node Build(Qontext context, Node node = null) {
			int step = 0;
			do {
				if (node == null)
					node = new Node();
				Token t = Peek();
				if (t.Check(ValueType.Keyword)) {
					if (t.Check(Keyword.REFERENCE)) {
						if (node.Left != null)
							throw new ParseException("can not declare a new reference here ._.", t);
						Type = SegmentType.REFERENCE;
						Digest();
						t = Peek();
						if (t.Check(ValueType.Identifier)) {
							Head.Left = new FloatingReference<string>(Digest().Str(), context);
						} else {
							throw new ParseException("expected identifier after new reference keyword, got '" + t + "' instead", t);
						}
					} else if (t.Check(Keyword.CURRENT_CONTEXT) || t.Check(Keyword.PARENT_CONTEXT)) {
						if (Head.Empty()) {
							Head = new Node(RRR(context), null, null);
						} else {
							node.Put(RRR(context));
						}
					} else if (t.Check(Keyword.RETURN)) {
						Digest();
						Returning = true;
					} else {
						throw new Exception("check me");
					}
				} else if (t.Check(ValueType.Operator)) {
					Operator op = Digest().GetValue<Operator>();
					// ToDo: Operator ==, < und >
					if (op.Symbol == Operator.ASSIGN_VALUE || op.Symbol == Operator.ASSIGN_REFERENCE) {
						if (step == 0)
							Returning = true;
						else if (Head.Empty() && step == 1) {
							Head.Left = node.Left;
							node.Left = null;
						}
					}
					if (node.Ready()) {
						Log.Spam("comparing operators to chain nodes");
						if (op.Compare(node.Operator) > 0) {
							node.Right = Build(context, new Node(node.Right, null, op));
						} else {
							node = Build(context, new Node(node, null, op));
						}
					} else if (Head.Operator == null && (
						op.Symbol == Operator.ASSIGN_VALUE ||
						op.Symbol == Operator.ASSIGN_REFERENCE)) {
						Head.Operator = op;
					} else if (node.Left == null || (node.Operator != null && node.Right == null)) {
						if (op.Symbol == Operator.CALCULATE_ADD)
							continue;
						else if (op.Symbol == Operator.CALCULATE_SUBTRACT) {
							var neg = new Segment(new Token[] {
								Token.Create(ValueType.Number, "-1"),
								Token.Create(ValueType.Operator, Operator.CALCULATE_MULTIPLY),
								Digest() });
							Log.Spam("created negating segment: " + neg.ToString());
							if (node.Left == null) {
								node.Left = neg.Execute(context);
							} else {
								node.Right = neg.Execute(context);
							}
						} else if (op.Symbol == Operator.CONDITION_BIGGER
							|| op.Symbol == Operator.CONDITION_BIGGER_EQUAL
							|| op.Symbol == Operator.CONDITION_SMALLER
							|| op.Symbol == Operator.CONDITION_SMALLER_EQUAL
							|| op.Symbol == Operator.CONDITION_EQUALS
							|| op.Symbol == Operator.CONDITION_NOT_EQUALS
						) {
							var tempContext = context;
							if (this.stack[0].IsType(ValueType.Identifier)) {
								node.Left = tempContext.Get(this.stack[0].Value.ToString());
							} else {
								node.Left = this.stack[0];
							}
							if (this.stack[2].IsType(ValueType.Identifier)) {
								node.Right = tempContext.Get(this.stack[2].Value.ToString());
							} else {
								node.Right = this.stack[2];
							}
							node.Operator = op;
							return node;
						} else {
							throw new ParseException("unexpected operator " + op.Symbol + " at start of expression", t);
						}
					} else if (node.Left != null) {
						node.Operator = op;
					} else {
						throw new ParseException("sorry, manipulation operators (operators without left-hand value) are not yet implemented. :/", t);
					}
				} else if (t.Check(ValueType.Struqture, "(")) {
					if (node.Ready())
						throw new ParseException("can not open new segment without prior operator.", t);
					Segment sub = new Segment(ReadBody());
					node.Put(sub.Execute(context));
				} else {
					if (t.Check(";") || EndOfStack()) {
						Log.Spam("end of node chain reached: ';'");
						Digest();
						break;
					}
					if (node.Ready())
						throw new ParseException("can not add another value to finished new segment node without prior operator.", t);
					QValue v = ReadNextValue(context);
					node.Put(v);
				}
				step++;
			} while (!EndOfStack());
			return node;
		}

		public override string ToString() {
			string r = "";
			foreach (Token t in stack) {
				r += " " + t.Str();
			}
			if (r.Length > 0)
				r = r[1..] + ";";
			return r;
		}

		protected class Node
		{

			public object Left { get; set; }
			public object Right { get; set; }
			public Operator Operator { get; set; }

			public Node(object left = null, object right = null, Operator op = null) {
				this.Left = left;
				this.Right = right;
				this.Operator = op;
			}

			public QValue Execute() {
				if (Left != null) {
					if (Operator != null && Right != null) {
						QValue _left;
						QValue _right;
						if (Right is Node) {
							_right = (Right as Node).Execute();
						} else {
							_right = (QValue) Right;
						}
						if (Left is Node) {
							_left = (Left as Node).Execute();
						} else {
							_left = (QValue) Left;
						}
						return Operator.Execute(_left, _right);
					} else {
						return (QValue) Left;
					}
				} else {
					return QValue.Null;
				}
			}

			public bool IsReturner() {
				return (Left == null && Right != null &&
					(Operator.Symbol == Operator.ASSIGN_VALUE || Operator.Symbol == Operator.ASSIGN_REFERENCE));
			}

			public bool Ready() {
				return (Left != null && Right != null && Operator != null);
			}

			public bool Empty() {
				return (Left == null && Right == null && Operator == null);
			}

			public bool Put(object value) {
				if (Left == null) {
					Left = value;
				} else if (Right == null) {
					Right = value;
				} else {
					return false;
				}
				return true;
			}

			public override string ToString() {
				if (Empty()) {
					return "{ void }";
				}
				return "{ " + (Left ?? "null") + " " + (Operator == null ? "NaO" : Operator.Symbol) + " " + (Right ?? "null") + " }";
			}
		}
	}

	internal class LoopSegment : Segment
	{

		public const int HEAD = 0x1, FOOT = 0x2;

		public Segment[] Body { get; protected set; }
		public int LoopType { get; protected set; }

		public LoopSegment(Segment[] body, int loopType, Token[] stack) : base(stack) {
			this.Body = body;
			this.LoopType = loopType;
		}

		public override QValue Execute(Qontext context) {
			QValue p = new QValue(true, ValueType.Boolean);
			do {
				if (LoopType == HEAD) {
					p = base.Execute(context);
					while (p.IsType(ValueType.Reference)) {
						p = (QValue) p.Value;
					}
				}
				if (!p.IsType(ValueType.Boolean))
					throw new ConditionException("expression for loop condition has to return a value of type BOOL, got " + p.Type.ToString() + " instead.");
				if (p.GetValue<bool>()) {
					Funqtion fq = new Funqtion(context, new List<Segment>(Body));
					QValue v = fq.Execute();
					if (fq.Returning) {
						this.Returning = true;
						return v;
					}
				} else {
					break;
				}
				if (LoopType == FOOT) {
					p = base.Execute(context);
					while (p.IsType(ValueType.Reference)) {
						p = (QValue) p.Value;
					}
				}
			} while (true);
			return null;
		}
	}

	internal class IfElseSegment : Segment
	{

		public Segment[] Body { get; protected set; }
		public Segment Next { get; protected set; }

		public IfElseSegment(Segment[] body, Token[] stack) : base(stack, SegmentType.CONDITION) {
			this.Body = body;
		}

		public void Append(IfElseSegment next) {
			this.Next = next;
		}

		public override QValue Execute(Qontext context) {
			QValue r = stack.Length == 0 ? QValue.True : base.Execute(context);
			while (r.IsType(ValueType.Reference)) {
				r = (QValue) r.Value;
			}
			if (!r.IsType(ValueType.Boolean))
				throw new ConditionException("expression for loop condition has to return a value of type Boolean, got " + r.Type.ToString() + " instead.");
			else if (r.GetValue<bool>()) {
				Funqtion xfq = new Funqtion(context, new List<Segment>(Body));
				QValue v = xfq.Execute();
				if (xfq.Returning)
					this.Returning = true;
				return v;
			} else if (Next != null) {
				Next.Execute(context);
			}
			return null;
		}
	}
}
