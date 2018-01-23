using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    internal class Segmentizer : Interpretoken
    {
        public Segmentizer(Token[] stack) : base(stack) { }

        public static Segment[] parse(Funqtion context, Token[] stack) {
            return new Segmentizer(stack).parse(context);
        }

        /**
         *  MAKE SEGMENTIZER -> SPLITS SEGMENTS BY ';' AND DETECTS CONDITION-, CLASS-, FUNCTION-, (etc...) -DEFINTIONS 
         * 
         * */

        public Segment[] parse(Funqtion context) {
            Log.spam("Statementizer.execute()");
            if (stack.Length == 0) return new Segment[0];
            List<Token> buffer = new List<Token>();
            List<Segment> segments = new List<Segment>();
            int level = 0;
            do {
                Token t = digest();

                Log.spam(t.ToString());
                if (t.check(";") && level == 0) {
                    segments.Add(new Segment(buffer.ToArray()));
                    buffer.Clear();
                } else {
                    if (t.check(Context.CHAR_OPEN)) level++;
                    else if (t.check(Context.CHAR_CLOSE)) level--;
                    buffer.Add(t);
                    if (endOfStack()) segments.Add(new Segment(buffer.ToArray()));
                }
            } while (!endOfStack());
            return segments.ToArray();
        }
    }
}
