using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    internal class Statementizer : Interpretoken
    {
        public Statementizer(Token[] stack) : base(stack) { }

        public static Statement[] parse(Funqtion context, Token[] stack) {
            return new Statementizer(stack).parse(context);
        }

        public Statement[] parse(Funqtion context) {
            Log.spam("Statementizer.execute()");
            if (stack.Length == 0) return new Statement[0]; 
            List<Token> buffer = new List<Token>();
            List<Statement> statements = new List<Statement>();
            int level = 0;
            do {
                Token t = digest();
                Log.spam(t.ToString());
                if (t.check(";") && level == 0) {
                    statements.Add(new Statement(buffer.ToArray()));
                    buffer.Clear();
                } else {
                    if (t.check(Context.CHAR_OPEN)) level++;
                    else if (t.check(Context.CHAR_CLOSE)) level--;
                    buffer.Add(t);
                    if (endOfStack()) statements.Add(new Statement(buffer.ToArray()));
                }
            } while (!endOfStack());
            return statements.ToArray();
        }
    }
}
