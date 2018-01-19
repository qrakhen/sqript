using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    internal class Interpretoken : Digester<Token>
    {
        protected const int OPEN = 0x0, CLOSE = 0x1, DEL = 0x2;

        public Interpretoken(Token[] stack) : base(stack) { }

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

        public Token[] remaining() {
            List<Token> buffer = new List<Token>();
            while (!endOfStack()) { 
                buffer.Add(digest());
            } 
            return buffer.ToArray();
        }

        public Token[] readBody(bool includeBrackets = false, string until = "") {
            List<Token> buffer = new List<Token>();
            int depth = 1;
            if (includeBrackets) buffer.Add(peek());
            string
                ascend = digest().str(),
                descend = "";
            if (until != "") descend = until;
            else switch (ascend) {
                case "~(": descend = ")"; break;
                case "(": descend = ")"; break;
                case "{": descend = "}"; break;
                case "[": descend = "]"; break;
                case "<": descend = ">"; break;
                default: throw new ParseException("could not find closing element for opened '" + ascend + "'", peek());
            } do {
                string cur = (peek().type == ValueType.STRUCTURE ? peek().getValue<string>() : "");
                if (cur == descend) depth--;
                else if (cur == ascend) depth++;
                if (depth > 0) buffer.Add(digest());
                else if (depth == 0)
                    if (includeBrackets) buffer.Add(digest());
                    else digest();
            } while (!endOfStack() && depth > 0);
            return buffer.ToArray();
        }

        /// <summary>
        /// resRefrec
        /// resolveReferenceRecursively :D
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Reference rrr(Context context, int adjust = 0, int __l = 0) {
            if (adjust != 0) shift(adjust);
            Log.spam("rrr inbound,\n __level: " + __l);
            Reference r = null;
            Token t = peek();
            string identifier;
            if (
                   (Keywords.isAlias(t.str(), Keyword.CURRENT_CONTEXT) && __l == 0) ||
                    Keywords.isAlias(t.str(), Keyword.PARENT_CONTEXT) ||
                    t.check(ValueType.IDENTIFIER)) { 
                identifier = digest().str();
                r = context.get(identifier);
                Log.spam("found " + (r == null ? " nothing, lol." : r.getReference()?.str()));
            } else throw new ParseException("tried to resolve reference with unexpected token '" + t + "'", t);
            if (r == null && __l == 0) r = context.lookupOrThrow(identifier);

            t = peek();
            if (t.check(Struqture.MEMBER)) {
                if (r?.getReference() is Context) {
                    digest(); // <~ into the void!
                    return rrr((Context)r.getReference(), 0, ++__l);
                } else throw new ParseException("tried to access member of a value that is not a context and thus memberless.", t);
            } else {
                if (r == null && identifier != null) {
                    Log.spam("rrr detected undeclared identifier at the end of the member chain,\nreturning floating reference for possible binding...");
                    r = new FloatingReference(identifier, context);
                }
                return r;
            }
        }

        public class FloatingReference : Reference
        {
            public string name { get; private set; }
            public Context owner { get; private set; }

            public FloatingReference(string name, Context owner) : base(NULL) {
                this.name = name;
                this.owner = owner;
            }

            public void bind() {
                owner.set(name, new Reference(value));
            }
        }
    }
}
