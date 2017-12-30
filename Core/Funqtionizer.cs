using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Funqtionizer : Interpretoken
    {
        public Funqtionizer(Token[] stack) : base(stack) { }

        public Statement[] parse(Funqtion context) {
            Debug.spam("Funqtionizer.execute()");
            if (stack.Length == 0) return new Statement[0]; 
            List<Token> buffer = new List<Token>();
            List<Statement> statements = new List<Statement>();
            do {
                Token cur = peek();
                Debug.spam(cur.ToString());
                if (cur.type == ValueType.STRUCTURE && cur.getValue<string>() == ";") {
                    digest();
                    statements.Add(new Statement(buffer.ToArray()));
                    buffer.Clear();
                } else if (cur.type == ValueType.KEYWORD) {
                    string keyword = Keywords.get(cur.getValue().ToString()).name;
                    if (keyword == Keyword.FUNQTION) {
                        digest();
                        Funqtion funqtion = new Funqtion(context);
                        if (peek().type != ValueType.IDENTIFIER) throw new FunqtionizerException("unexpected token when trying to parse funqtion definition, expected name", peek());
                        string name = digest().getValue<string>();
                        Debug.spam("creating new funqtion definition '" + name + "'");
                        Token t = digest();
                        if (t.type == ValueType.STRUCTURE && t.getValue<string>() == "(") {
                            do {
                                t = digest();
                                if (t.type == ValueType.IDENTIFIER) {
                                    funqtion.parameters.Add(t.getValue<string>());
                                    t = digest();
                                    if (t.type == ValueType.STRUCTURE && t.getValue<string>() == ",") continue;
                                    else if (t.type == ValueType.STRUCTURE && t.getValue<string>() == ")") break;
                                    else throw new FunqtionizerException("unexpected token found when trying to parse funqtion parameter declaration", t);
                                } else throw new FunqtionizerException("unexpected token found when trying to parse funqtion parameter declaration", t);
                            } while (!endOfStack());
                            if (endOfStack()) throw new FunqtionizerException("unexpected end of stack when trying to parse funqtion parameter declaration", t);
                            else {
                                t = peek();
                                if (t.type == ValueType.STRUCTURE && t.getValue<string>() == "{") {
                                    Token[] body = readBody();
                                    funqtion.statements.AddRange(new Funqtionizer(body).parse(funqtion));
                                    context.set(name, new Reference(funqtion));
                                    Debug.spam("funqtion " + name + " parsed:\n" + funqtion.ToString());
                                } else throw new FunqtionizerException("unexpected funqtion body opening, expected '{'", t);
                            }
                        } else throw new FunqtionizerException("unexpected funqtion parameter opening, expected '('", t);
                    } else {
                        buffer.Add(digest());
                        if (endOfStack()) statements.Add(new Statement(buffer.ToArray()));
                    }
                } else {
                    buffer.Add(digest());
                    if (endOfStack()) statements.Add(new Statement(buffer.ToArray()));
                }
            } while (!endOfStack());
            return statements.ToArray();
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

    public class FunqtionizerException : Exception
    {
        public FunqtionizerException(string message, Token cause = null) : base(message, cause) { }
    }
}
