using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    internal class Statement : Interpretoken
    {
        public Statement(Token[] stack) : base(stack) { }

        public Value execute(Funqtion context, bool forceReturn = false) {
            reset();
            Log.spam("executing statement:\n" + ToString());
            Reference target = null;
            Value result = null;
            Condition condition = null;
            string identifier = "";
            bool
                declaring = false,
                returning = false;
            do {
                Token t = peek();
                if (t.check(ValueType.KEYWORD)) {
                    Keyword keyword = digest().getValue<Keyword>();
                    switch (keyword.name) {
                        case Keyword.REFERENCE: declaring = true; break;
                        case Keyword.FUNQTION: declaring = true; break; 
                        case Keyword.QLASS: declaring = true; break; 
                        case Keyword.CONDITION_IF: condition = Conditionizer.parse(keyword, context, remaining()); break;
                        case Keyword.CONDITION_LOOP: condition = Conditionizer.parse(keyword, context, remaining()); break;
                        case Keyword.RETURN: returning = true; break;
                        case Keyword.CURRENT_CONTEXT: target = rrr(context, -1); break;
                        case Keyword.PARENT_CONTEXT: target = rrr(context, -1); break;
                        default: throw new Exception("unexpected or not yet supported keyword '" + keyword.name + "'", peek());
                    }
                    if (declaring) target = new Reference();
                } else if (t.type == ValueType.IDENTIFIER) {
                    if (target == null) {
                        target = rrr(context); //context.query(identifier, false, false);
                        result = target;
                    } else if (declaring) {
                        context.set(digest().str(), target);
                    } else throw new Exception("unexpected identifier or context query '" + peek().str() + "'", t);
                } else if (t.check(Struqture.Funqtion[OPEN])) {
                    if (declaring) {
                        Log.spam("expecting funqtion declaration next");
                        Funqtion fq = Funqtionizer.parse(context, readBody(true));
                        target.assign(fq);
                        result = target;
                    } else {
                        Log.spam("expecting funqtion call next");
                        if (!target.getReference().isType(ValueType.FUNQTION)) throw new ParseException("trying to execute a non-funqtion", t);
                        Value[] parameters = Funqtionizer.parseParameters(context, readBody(true));
                        result = (target.getReference() as Funqtion).execute(parameters);
                    }
                } else if (t.check(Operator.ASSIGN_REFERENCE) || t.check(Operator.ASSIGN_VALUE)) {
                    // if a <~ or <& is detected right at the beginning of the statement,
                    // we treat it like a return statement. sqript rules.
                    if (position == 0) {
                        returning = true;
                        target = new Reference();
                    } else if (target == null) throw new Exception("assign? assign to WHAT? there's no target reference. ._.");

                    Operator op = digest().getValue<Operator>();
                    Value value = null;
                    position += Vactory.readNextValue(context, remaining(), out value);
                    if (value == null) throw new ParseException("could not read value to be " + (returning ? "returned" : "assigned to '" + identifier + ".") + ": unreadable or no value", t);

                    op.execute(target, value);

                    /*
                    Operator op = digest().getValue<Operator>();
                    Token[] right = new Token[(stack.Length - position)];
                    for (int i = 0; i < right.Length; i++) right[i] = digest();

                    Expression expr;
                    if (right.Length == 1) expr = new Expression(op, target, digest(), context);
                    else expr = new Expression(op, target, new Expressionizer(right).parse(context), context);
                    result = expr.execute();*/
                } else if (t.isType(ValueType.ANY_VALUE) || t.check("(")) {
                    Token[] remaining = new Token[(stack.Length - position)];
                    for (int i = 0; i < remaining.Length; i++) remaining[i] = digest();
                    Expression expr = new Expressionizer(remaining).parse(context);
                    result = expr.execute();
                } else Log.warn("unexpected token: '" + digest() + "'");
            } while (!endOfStack());
            Log.spam("statement result:\n - target: " + target?.ToString() + "\n - result: " + result?.ToString());
            if (target is FloatingReference) (target as FloatingReference).bind();

            if (condition != null) return condition.execute();
            
            if (returning) return result;
            else if (forceReturn) return (result == null ? target : result);
            else return null;
        }

        private Reference getReference(Context context, string name) {
            return context.lookupOrThrow(name);
        }

        public override string ToString() {
            string str = "";
            foreach (Token token in stack) {
                str += token.getValue().ToString() + " ";
            }
            return str;
        }
    }
}
