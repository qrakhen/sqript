﻿using System;
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

        public Expression _parse(Context context) {
            Expression prev = null;
            Expr buffer = new Expr();
            do {
                Token t = digest();
                if (t.type == ValueType.OPERATOR) {
                    if (prev != null) buffer.left = prev;
                    else if (buffer.left == null) throw new Exception("manipulation operators (-1, i++) are not yet implemented. thank you for your patience.", t);
                    else buffer.op = t.getValue<Operator>();
                } else if (t.type == ValueType.STRUCTURE) {
                    if (t.getValue<string>() == "(") {
                        object sub = parse(context).execute();
                        if (buffer.left == null) buffer.left = sub;
                        else if (buffer.right == null) buffer.right = sub;
                    } else if (t.getValue<string>() == ")") {
                        return (prev == null ? new Expression(buffer.op, buffer.left, buffer.right, context) : prev);
                    } else if (t.getValue<string>() == "[") {
                        Array array = new Array();
                        do {
                            t = digest();
                            if (t.type == ValueType.STRUCTURE && t.getValue<string>() == "]") break;
                            array.add(t);
                        } while (!endOfStack());
                        if (buffer.left == null) buffer.left = array;
                        else if (buffer.right == null) buffer.right = array;
                    } else if (t.getValue<string>() == "{") {
                        Obqect obqect = new Obqect(context);
                        do {
                            t = digest();
                            if (t.type == ValueType.STRUCTURE && t.getValue<string>() == "}") break;
                            else if (t.type == ValueType.IDENTIFIER) {
                                string key = t.getValue<string>();
                            }
                        } while (!endOfStack());
                        if (buffer.left == null) buffer.left = obqect;
                        else if (buffer.right == null) buffer.right = obqect;
                    }
                } else {
                    if (buffer.left == null) buffer.left = t;
                    else if (buffer.right == null) buffer.right = t;
                }
                //if (buffer.right == null && endOfStack()) buffer.right = 
                if (buffer.ready() || endOfStack()) {
                    prev = new Expression(buffer.op, buffer.left, buffer.right, context);
                    buffer = new Expr();
                }
            } while (!endOfStack());
            return prev;
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
                    if (peek().check(Funqtionizer.FQ_OPEN) && (r.getReference().isType(ValueType.FUNQTION))) {
                        Value[] p = Funqtionizer.parseParameters(context, readBody(true));
                        v = (r.getReference() as Funqtion).execute(p);
                    } else v = r.getReference();
                } else if (t.check(ValueType.KEYWORD)) {

                } else if (t.check(ValueType.STRUCTURE)) {
                    if (t.check("(")) {
                        object sub = parse(context).execute();
                        if (expr.left == null) expr.left = sub;
                        else if (expr.right == null) expr.right = sub;
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