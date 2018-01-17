using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    internal class Interpretoken : Digester<Token>
    {
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
                case "(": descend = ")"; break;
                case ":(": descend = ")"; break;
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
        public Reference resRefrec(Context context, int adjust = 0) {
            if (adjust != 0) shift(adjust);
            Log.spam("RESREFREC inbound");
            Reference r = null;
            Token t = peek();
            if (
                    t.check(ValueType.IDENTIFIER) ||
                    t.check(Keyword.CURRENT_CONTEXT) ||
                    t.check(Keyword.PARENT_CONTEXT)) {

                r = context.get(digest().str());
                Log.spam("RESREFREC found " + r.getReference()?.str());
            } else throw new ParseException("tried to resolve reference with unexpected token '" + t + "'", t);

            t = peek();
            if (t.check(Structure.MEMBER_KEY_DELIMITER)) { 
                if (r?.getReference() is Context) {
                    digest(); // <~ into the void!
                    return resRefrec((Context)r.getReference());
                } else throw new ParseException("tried to access member of a value that is not a context and thus memberless.", t);
            } else return r;
        }
    }
}
