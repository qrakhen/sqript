using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Statement : Interpretoken
    {
        public Statement(Token[] stack) : base(stack) { }

        public Value execute(Funqtion context, bool forceReturn = false) {
            Debug.spam("executing statement:\n" + ToString());
            Reference target = null;
            Value result = null;
            bool
                declaring = false,
                returning = false;
            do {
                Token t = peek();
                if (t.check(ValueType.KEYWORD)) {
                    Keyword keyword = digest().getValue<Keyword>();
                    switch (keyword.name) {
                        case Keyword.DECLARE: target = declareReference(context, digest().str()); break;
                        case Keyword.FUNQTION: target = declareReference(context, digest().str()); break;
                        case Keyword.QLASS: target = declareReference(context, digest().str()); break;
                        case Keyword.RETURN: returning = true; break;
                        default: throw new Exception("unexpected or not yet supported keyword '" + keyword.name + "'", peek());
                    }
                    if (target != null) declaring = true;
                } else if (t.type == ValueType.IDENTIFIER) {
                    if (target == null) target = getReference(context, digest().getValue<string>());
                    if (peek().check(Structure.MEMBER_KEY_DELIMITER)) {
                        digest();
                        List<object> buffer = new List<object>();
                        do {
                            if (peek().type == ValueType.IDENTIFIER && target.getValueType() == ValueType.OBQECT) {
                                string key = digest().getValue<string>();
                                Reference r = (target.getReference() == null ? null : (target.getReference() as Obqect).get(key));
                                if (r == null) {
                                    r = new Reference(null);
                                    (target.getReference() as Obqect).set(key, r);
                                }
                                target = r;
                            } else if (peek().isType((int)ValueType.NUMBER) && target.getValueType() == ValueType.ARRAY) {
                                int key = digest().getValue<int>();
                                Reference r = (target.getReference() == null ? null : (target.getReference() as Array).get(key));
                                if (r == null) {
                                    r = new Reference(null);
                                    (target.getReference() as Array).set(key, r);
                                }
                                target = r;
                            } else {
                                throw new Exception("unexpected token when trying to resolve member tree", peek());
                            }
                            if (peek().type == ValueType.STRUCTURE && peek().getValue<string>() == Structure.MEMBER_KEY_DELIMITER) {
                                digest();
                                continue;
                            } else break;
                        } while (!endOfStack());
                        //select = new Reference.MemberSelect(target, buffer.ToArray()); 
                    }
                } else if (t.check(Funqtionizer.FQ_OPEN)) {
                    if (declaring) {
                        Funqtion fq = Funqtionizer.parse(context, readBody(true));
                        target.assign(fq, true);
                        result = target;
                    } else {
                        if (!target.getReference().isType(ValueType.FUNQTION)) throw new ParseException("trying to execute a non-funqtion", t);
                        Value[] parameters = Funqtionizer.parseParameters(context, readBody(true));
                        result = (target.getReference() as Funqtion).execute(parameters);
                    }
                } else if (t.type == ValueType.OPERATOR) {
                    Operator op = digest().getValue<Operator>();
                    Token[] right = new Token[(stack.Length - position)];
                    for (int i = 0; i < right.Length; i++) right[i] = digest();

                    Expression expr;
                    if (right.Length == 1) expr = new Expression(op, target, digest(), context);
                    else expr = new Expression(op, target, new Expressionizer(right).parse(context), context);
                    result = expr.execute();
                } else if (t.isType(ValueType.ANY_VALUE)) {
                    Token[] remaining = new Token[(stack.Length - position)];
                    for (int i = 0; i < remaining.Length; i++) remaining[i] = digest();
                    Expression expr = new Expressionizer(remaining).parse(context);
                    result = expr.execute();
                } else Debug.warn("unexpected token: '" + digest() + "'");
            } while (!endOfStack());
            resetStack();
            if (returning || forceReturn) return result;
            else return null;
        }

        private Reference declareReference(Context context, string name) {
            Reference r = new Reference();
            context.set(name, r);
            Debug.spam("reference '" + name + "' declared!");
            return r;
        }

        private Reference getReference(Context context, string name) {
            return context.lookupOrThrow(name);
        }

        public override string ToString() {
            string str = "";
            foreach (Token token in stack) {
                str += token.getValue().ToString() + " ";
            } 
            return str.Substring(0, str.Length - 1) + ";";
        }
    }
}
