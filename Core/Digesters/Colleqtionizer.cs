namespace Qrakhen.Sqript
{
    internal class Colleqtionizer : Interpretoken
    {
        public Colleqtionizer(Token[] stack) : base(stack) { }
        
        public static Obqect readObqect(Qontext context, Token[] stack) { return new Colleqtionizer(stack).readObqect(context); }

        public Obqect readObqect(Qontext context) {
            Log.spam("Colleqtionizer.readObqect()");
            Obqect o = new Obqect(context);
            if (stack.Length == 0) return o;
            Token t = digest();
            if (t.check(Struqture.Context[OPEN])) {
                do {
                    t = peek();
                    if (t.check(ValueType.Identifier)) {
                        string key = digest().str();
                        t = peek();
                        if (t.check(Operator.ASSIGN_REFERENCE) || t.check(Operator.ASSIGN_VALUE)) {
                            Operator op = Operators.get(digest().ToString());
                            Value value = readNextValue(context);
                            if (value == null) throw new ParseException("could not read value for '" + key + "': unreadable or no value.", t);
                            if (value is Reference) {
                                if (op.symbol == Operator.ASSIGN_REFERENCE) o.set(key, new Reference(value));
                                else o.set(key, new Reference((value as Reference).getTrueValue()));
                            } else {
                                o.set(key, new Reference(value));
                            }
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

        public static Array readArray(Qontext context, Token[] stack) { return new Colleqtionizer(stack).readArray(context); }

        public Array readArray(Qontext context) {
            Log.spam("Colleqtionizer.readArray()");
            Array a = new Array();
            if (stack.Length == 0) return a;
            Token t = digest();
            if (t.check(Struqture.Collection[OPEN])) {
                do {
                    t = peek();
                    int index = -1;
                    Operator op = Operators.get(Operator.ASSIGN_VALUE);
                    if (t.check(ValueType.Integer)) {
                        if (peek(1).check(Operator.ASSIGN_VALUE) || peek(1).check(Operator.ASSIGN_REFERENCE)) {
                            index = digest().getValue<int>();
                            op = digest().getValue<Operator>();
                        }
                    }

                    if (t.check(Struqture.Collection[CLOSE])) {
                        digest();
                        break;
                    } else {
                        Value value = readNextValue(context);
                        if (value == null) throw new ParseException("could not read value for array: unreadable or no value.", t);
                        if (value is Reference) {
                            if (op.symbol == Operator.ASSIGN_REFERENCE) a.set(index, new Reference(value));
                            else a.set(index, new Reference((value as Reference).getTrueValue()));
                        } else {
                            a.set(index, new Reference(value));
                        }
                        t = digest();
                        if (t.check(Struqture.Collection[DEL])) continue;
                        else if (t.check(Struqture.Collection[CLOSE])) break;
                        else throw new ParseException("unexpected token '" + t + "' when reading array body, expected ']' or ','", t);
                    }
                } while (!endOfStack());
            } else throw new ParseException("unexpected token '" + t + "' when reading array body, expected '['", t);
            return a;
        }
    }
}
