using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Statementizer : Interpretoken
    {
        public Statementizer(Token[] stack) : base(stack) { }

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
                } else {
                    buffer.Add(digest());
                    if (endOfStack()) statements.Add(new Statement(buffer.ToArray()));
                }
            } while (!endOfStack());
            return statements.ToArray();
        }
    }
}
