using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    internal class Expressionizer : Interpretoken
    {
        public Expressionizer(Token[] stack) : base(stack) { }

        private struct Expr
        {
            public object left, right;
            public Operator op;

            public Expr(object left = null, object right = null, Operator op = null) {
                this.left = left;
                this.right = right;
                this.op = op;
            }

            public bool ready() {
                return (left != null && right != null && op != null);
            }
        }

        public static Expression parse(Context context, Token[] stack) {
            return new Expressionizer(stack).parse(context);
        }

        public Expression parse(Context context) {
            Expression head = null;
            Expr expr = new Expr();
            do {
                Token t = peek();
                Value v = null;
                if (t.type == ValueType.OPERATOR) {
                    if (head != null) expr.left = head;
                    else if (expr.left == null) throw new Exception("manipulation operators (-1, i++) are not yet implemented. thank you for your patience.", t);
                    if (expr.right == null) expr.op = digest().getValue<Operator>();
                    else throw new Exception("unexpected operator after left and right token have been read: " + expr.left.ToString() + " " + expr.right.ToString(), t);
                } else {
                    v = readNextValue(context);
                    if (v == null) throw new Exception("expected any value while parsing expression, got '" + v + "' instead.");
                }

                if (v != null) {
                    if (expr.left == null) expr.left = v;
                    else if (expr.right == null) expr.right = v;
                }

                if (expr.ready() || endOfStack()) {
                    head = new Expression(expr.op, expr.left, expr.right, context);
                    expr = new Expr();
                }
            } while (!endOfStack());
            reset();
            return head;
        }
    }
}
