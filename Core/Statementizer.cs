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
            int level = 0;
            do {
                Token t = digest();
                Debug.spam(t.ToString());
                if (t.check(";") && level == 0) {
                    statements.Add(new Statement(buffer.ToArray()));
                    buffer.Clear();
                } else {
                    if (t.check(Context.CHAR_OPEN)) level++;
                    else if (t.check(Context.CHAR_OPEN)) level--;
                    buffer.Add(t);
                    if (endOfStack()) statements.Add(new Statement(buffer.ToArray()));
                }
            } while (!endOfStack());
            return statements.ToArray();
        }
    }
}
