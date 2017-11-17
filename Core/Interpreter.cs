using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Function : Context
    {
        public Value[] parameters { get; private set; }
        public Token[] stack { get; private set; }

        public Function(Context parent) : base(parent) {

        }
    }

    public class Class : Context
    {
        public Token[] stack { get; private set; }

        public Class(Context parent) : base(parent) {

        }
    }

    /// <summary>
    /// [MODIFIER] [KEYWORD] NAME  OPERATOR|FUNC_CALL|FUNC_HEADER OPERATION|BODY_DEFINITION;
    ///            ref       value =                              5 * (4 + 2);
    ///            function  add   (a, b)                         { return a + b; }
    /// </summary>
    public class Block
    {

    }

    public class ExpressionParser : Interpreter
    {
        public ExpressionParser(Context context, Token[] stack) : base(context, stack) { }

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

        public Expression parse() {
            Expression prev = null;
            Expr buffer = new Expr();
            do {
                Token t = digest();
                if (t.type == ValueType.OPERATOR) {
                    if (prev != null) buffer.left = prev;
                    else if (buffer.left == null) throw new Exception("manipulation operators (-1, i++, a()) are not yet implemented. thank you for your patience.", t);
                    else buffer.op = t.getValue<Operator>();
                } else if (t.type == ValueType.STRUCTURE) {
                    if (t.getValue<string>() == "(") {
                        object sub = parse().execute();
                        if (buffer.left == null) buffer.left = sub;
                        else if (buffer.right == null) buffer.right = sub;
                    } else if (t.getValue<string>() == ")") {
                        return (prev == null ? new Expression(buffer.op, buffer.left, buffer.right, context) : prev);
                    }
                } else {
                    if (buffer.left == null) buffer.left = t;
                    else if (buffer.right == null) buffer.right = t;
                }
                //if (buffer.right == null && endOfStack()) buffer.right = 
                if (buffer.ready()) {
                    prev = new Expression(buffer.op, buffer.left, buffer.right, context);
                    buffer = new Expr();
                }
            }  while (!endOfStack());
            return prev;
        }
    }

    public class Statement : Interpreter
    {
        public Statement(Context context, Token[] stack) : base(context, stack) { }

        public override void execute() {
            Reference target = null;
            Value result = null;
            do {
                Token t = peek();
                if (t.type == ValueType.KEYWORD) {
                    Keyword keyword = digest().getValue<Keyword>();
                    string identifier = peek().getValue<string>();
                    switch (keyword.name) {
                        case Keyword.DECLARE: target = declareReference(digest().getValue<string>()); break;
                        //case Keyword.FUNCTION: target = declareReference(digest().getValue<string>()); break;
                        default: throw new Exception("only reference creation implemented yet", peek());
                    }
                } else if (t.type == ValueType.IDENTIFIER) {
                    target = getReference(digest().getValue<string>());
                } else if (t.type == ValueType.OPERATOR) {
                    Operator op = digest().getValue<Operator>();
                    Token[] right = new Token[(stack.Length - position)];
                    for (int i = 0; i < right.Length; i++) right[i] = digest();
                    Expression expr;
                    if (right.Length == 1) expr = new Expression(op, target, digest(), context);
                    else expr = new Expression(op, target, new ExpressionParser(context, right).parse(), context);
                    result = expr.execute();
                } else Debug.warn("unexpected token: '" + digest() + "'");
            } while (!endOfStack());
            if (result != null) Debug.log(result.toDebug());
            else if (target != null) Debug.log(target.toDebug());
        }

        private Reference declareReference(string name) {
            Reference r = new Reference(name, null);
            context.createReference(r);
            Debug.spam("reference '" + name + "' declared!");
            return r;
        }

        private Reference getReference(string name) {
            Reference r = context.getReference(name);
            if (r == null) throw new NullReferenceException("unknown reference name '" + name + "' given");
            return r;
        }
    }

    public class Interpreter : Digester<Token> {
        protected Context context;

        public Interpreter(Context context, Token[] stack) : base(stack) {
            this.context = (context == null ? new Context(null) : context);
        }

        protected override Token digest() {
            Token t = base.digest();
            Runtime.reader.token = t;
            return t;
        }

        protected override Token peek() {
            Token t = base.peek();
            Runtime.reader.token = t;
            return t;
        }

        public virtual void execute() {
            List<Token> buffer = new List<Token>();
            Debug.spam("interpreter.execute()");
            do {
                Token cur = peek();
                Debug.spam(cur.ToString());
                if (cur.type == ValueType.STRUCTURE && cur.getValue<string>() == ";") {
                    digest();
                    new Statement(context, buffer.ToArray()).execute();
                    buffer.Clear();
                } else {
                    buffer.Add(digest());
                    if (endOfStack()) new Statement(context, buffer.ToArray()).execute();
                }
            } while (!endOfStack());
        }

        public Token[] readBody() {
            List<Token> buffer = new List<Token>();
            int depth = 1;
            string
                ascend = digest().getValue<string>(),
                descend = (ascend == "{" ? "}" : (ascend == "(" ? ")" : (ascend == "[" ? "]" : "")));
            if (descend == "") throw new ParseException("could not find closing element for opened '" + ascend + "'", peek());

            do {
                string cur = (peek().type == ValueType.STRUCTURE ? peek().getValue<string>() : "");
                if (cur == descend) depth--;
                else if (cur == ascend) depth++;
                if (depth > 0) buffer.Add(digest());
                else if (depth == 0) digest();
            } while (!endOfStack() && depth > 0);
            return buffer.ToArray();
        }
    }
}
