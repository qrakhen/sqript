using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Expressionizer : Interpretoken
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

        public Expression parse(Context context) {
            Expression head = null;
            Expr expr = new Expr();
            do {
                Token t = digest();
                Value v = null;
                if (t.type == ValueType.OPERATOR) {
                    if (head != null) expr.left = head;
                    else if (expr.left == null) throw new Exception("manipulation operators (-1, i++) are not yet implemented. thank you for your patience.", t);
                    else expr.op = t.getValue<Operator>();
                } else if (t.check(ValueType.IDENTIFIER)) {
                    Reference r = context.query(t.str());
                    if (peek().check(Funqtionizer.FQ_CALL_OPEN) && (r.getReference().isType(ValueType.FUNQTION))) {
                        Value[] p = Funqtionizer.parseParameters(context, readBody(true));
                        v = (r.getReference() as Funqtion).execute(p);
                    } else v = r.getReference();
                } else if (t.check(ValueType.KEYWORD)) {

                } else if (t.check(ValueType.STRUCTURE)) {
                    if (t.check(Funqtionizer.FQ_DECLARE_OPEN)) {
                        shift(-1);
                        Funqtion fq = Funqtionizer.parse(context, readBody(true));
                        v = fq;
                    } else if (t.check("(")) {
                        Value sub = parse(context).execute();
                        v = sub;
                    } else if (t.check(")")) {
                        return (head == null ? new Expression(expr.op, expr.left, expr.right, context) : head);
                    } else if (t.getValue<string>() == "[") {
                        Array array = new Array();
                        do {
                            t = digest();
                            if (t.type == ValueType.STRUCTURE && t.getValue<string>() == "]") break;
                            array.add(t);
                        } while (!endOfStack());
                        v = array;
                    } else if (t.getValue<string>() == "{") {
                        Obqect obqect = new Obqect(context);
                        do {
                            t = digest();
                            if (t.type == ValueType.STRUCTURE && t.getValue<string>() == "}") break;
                            else if (t.type == ValueType.IDENTIFIER) {
                                string key = t.getValue<string>();
                            }
                        } while (!endOfStack());
                        v = obqect;
                    }
                } else if (t.isType(ValueType.ANY_VALUE)) {
                    v = t.makeValue();
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
            return head;
        }
    }
}
