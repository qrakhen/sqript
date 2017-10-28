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

    public class Statement : Interpreter
    {
        public Statement(Context context, Token[] stack) : base(context, stack) { }

        public override void execute() {
            Reference target = null;
            do {
                Token t = peek();
                if (t.type == Value.Type.KEYWORD) {
                    Keyword keyword = digest().getValue<Keyword>();
                    string identifier = peek().getValue<string>();
                    switch (keyword.name) {
                        case Keyword.DECLARE: target = declareReference(digest().getValue<string>()); break;
                        default: throw new NotImplementedException("only reference creation implemented yet");
                    }
                } else if (t.type == Value.Type.IDENTIFIER) {
                    target = getReference(peek().getValue<string>());
                } else if (t.type == Value.Type.OPERATOR) {
                    if (Regex.IsMatch(t.getValue<string>(), "=")) {
                        if (target == null) throw new OperationException("can not execute assignment to no target reference");
                    }
                }
                
            } while (peek() != null);
        }

        private Reference declareReference(string name) {
            Reference r = new Reference(name, Value.Type.UNDEFINED, null);
            context.createReference(r);
            return r;
        }

        private Reference getReference(string name) {
            Reference r = context.getReference(name);
            if (r == null) throw new NullReferenceException("unknown reference name '" + name + "' given");
            return r;
        }
    }

    public class Interpreter : Digester<Token>
    {
        protected Context context;

        public Interpreter(Context context, Token[] stack) : base(stack) {
            this.context = (context == null ? new Context(null) : context);
        }

        public virtual void execute() {
            List<Token> buffer = new List<Token>();
            do {
                Token cur = peek();
                if (cur.type == Value.Type.STRUCTURE && cur.getValue<string>() == ";") {
                    new Statement(context, buffer.ToArray()).execute();
                    buffer.Clear();
                } else if (cur.type == Value.Type.STRUCTURE && Regex.IsMatch(cur.getValue<string>(), "[{[(]")) {
                    readBody();
                } else {
                    buffer.Add(digest());
                }
            } while (peek() != null);
        }

        public Token[] readBody() {
            List<Token> buffer = new List<Token>();
            int depth = 1;
            string
                descend = digest().getValue<string>(),
                ascend = (descend == "{" ? "}" : (descend == "(" ? ")" : (descend == "[" ? "]" : "")));
            if (ascend == "") throw new ParseException("could not find closing element for opened '" + descend + "'");

            do {
                string cur = peek().getValue<string>();
                if (cur == ascend) depth--;
                else if (cur == descend) depth++;
                if (depth > 0) buffer.Add(digest());
                else if (depth == 0) digest();
            } while (peek() != null && depth > 0);
            return buffer.ToArray();
        }
    }
}
