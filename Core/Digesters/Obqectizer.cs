namespace Qrakhen.Sqript
{
    internal class Obqectizer : Interpretoken
    {
        public Obqectizer(Token[] stack) : base(stack) { }

        public static Obqect parse(Qontext context, Token[] stack) {
            return new Obqectizer(stack).parse(context);
        }

        public Obqect parse(Qontext context) {
            Log.spam("Obqectizer.parse()");
            Obqect o = new Obqect(context);
            if (stack.Length == 0) return o;
            Token t = digest();
            if (t.check(Struqture.Context[OPEN])) {
                do {
                    t = peek();
                    if (t.check(ValueType.IDENTIFIER)) {
                        string key = digest().str();
                        t = peek();
                        if (t.check(Operator.ASSIGN_REFERENCE) || t.check(Operator.ASSIGN_VALUE)) {
                            Operator op = Operators.get(digest().ToString());
                            Value value = readNextValue(context);
                            if (value == null) throw new ParseException("could not read value for '" + key + "': unreadable or no value.", t);
                            o.set(key, new Reference(value));
                            t = digest();
                            if (t.check(Struqture.Context[DEL])) continue;
                            else if (t.check(Struqture.Context[CLOSE])) break;
                            else throw new ParseException("unexpected token '" + t + "' when reading obqect body, expected '}' or ','", t);
                        } else throw new ParseException("expected assign_value/reference for the next token, got '" + t + "' instead.", t);
                    } else if (t.check(Struqture.Context[CLOSE])) {
                        digest();
                        break;
                    } else throw new ParseException("expected identifier as obqect key, got '" + t + "' instead.", t);
                } while (!endOfStack());
            } else throw new ParseException("unexpected token '" + t + "' when reading obqect body, expected '{'", t);
            return o;
        }
    }
}
