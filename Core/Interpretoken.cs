using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Interpretoken : Digester<Token>
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

        public Token[] readBody(bool includeBrackets = false) {
            List<Token> buffer = new List<Token>();
            int depth = 1;
            if (includeBrackets) buffer.Add(peek());
            string
                ascend = digest().str(),
                descend = "";
            switch (ascend) {
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
    }
}
