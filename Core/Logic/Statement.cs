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
            Condition condition = null;
            string 
                declaredName = "",
                identifier = "";
            bool
                declaring = false,
                returning = false;
            do {
                Token t = peek();
                if (t.check(ValueType.KEYWORD)) {
                    Keyword keyword = digest().getValue<Keyword>();
                    switch (keyword.name) {
                        case Keyword.REFERENCE: target = new Reference(); break;
                        case Keyword.FUNQTION: target = new Reference(); break; 
                        case Keyword.QLASS: target = new Reference(); break; 
                        case Keyword.CURRENT_CONTEXT: target = new Reference(); break;
                        case Keyword.CONDITION_IF: condition = Conditionizer.parse(keyword, context, remaining()); break;
                        case Keyword.CONDITION_LOOP: condition = Conditionizer.parse(keyword, context, remaining()); break;
                        case Keyword.RETURN: returning = true; break;
                        default: throw new Exception("unexpected or not yet supported keyword '" + keyword.name + "'", peek());
                    }
                    if (target != null) declaring = true;
                } else if (t.type == ValueType.IDENTIFIER) {
                    identifier = digest().str();
                    if (target == null) {
                        target = context.query(identifier, false, false);
                        result = target;
                    } else if (declaring && !identifier.Contains(Context.MEMBER_DELIMITER)) declaredName = identifier;
                    else throw new Exception("unexpected identifier or context query '" + identifier + "'", t);
                } else if (t.check(Funqtionizer.FQ_DECLARE_OPEN)) {
                    if (declaring) {
                        Funqtion fq = Funqtionizer.parse(context, readBody(true));
                        target.assign(fq, true);
                        result = target;
                    } else {
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
                    }
                    // assignment operators need to be treated in a special way like this, 
                    // there's no other way i could think of and yes, i thought about that a lot.
                    // i deemed consistency among the rest of my code as more important.
                    if (target == null) target = context.query(identifier, false);
                    if (target == null) target = context.lookupOrThrow(identifier);

                    Operator op = digest().getValue<Operator>();
                    Token[] right = new Token[(stack.Length - position)];
                    for (int i = 0; i < right.Length; i++) right[i] = digest();

                    Expression expr;
                    if (right.Length == 1) expr = new Expression(op, target, digest(), context);
                    else expr = new Expression(op, target, new Expressionizer(right).parse(context), context);
                    result = expr.execute();
                } else if (t.isType(ValueType.ANY_VALUE) || t.check("(")) {
                    Token[] remaining = new Token[(stack.Length - position)];
                    for (int i = 0; i < remaining.Length; i++) remaining[i] = digest();
                    Expression expr = new Expressionizer(remaining).parse(context);
                    result = expr.execute();
                } else Debug.warn("unexpected token: '" + digest() + "'");
            } while (!endOfStack());

            reset();

            if (condition != null) return condition.execute();

            if (declaring) context.set(declaredName, target);
            
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
