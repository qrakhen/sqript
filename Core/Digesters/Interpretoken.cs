using System;
using System.Collections.Generic;

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
                string cur = (peek().type == ValueType.Struqture ? peek().getValue<string>() : "");
                if (!string.IsNullOrEmpty(cur) && descend.IndexOf(cur) > -1) depth--;
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
        public Reference[] rrra<T>(Collection<T> context, int __l = 0, List<Reference> __buffer = null) {
            if (__buffer == null) __buffer = new List<Reference>();
            Log.spam("rrr inbound,\n __level: " + __l);
            Reference r = null;
            Token t = peek();
            T key;
            if (
                   (Keywords.isAlias(t.str(), Keyword.CURRENT_CONTEXT) && __l == 0) ||
                    Keywords.isAlias(t.str(), Keyword.PARENT_CONTEXT) ||
                    t.check(ValueType.Identifier) || t.check(ValueType.Integer)) { 
                key = (t.getValue() is Keyword ? digest().getValue<Keyword>().getKey<T>() : digest().getValue<T>());
                r = context.get(key);
                __buffer.Add(r);
                Log.spam("found " + (r == null ? " nothing, lol." : r.getTrueValue()?.str()));
            } else throw new ParseException("tried to resolve reference with unexpected token '" + t + "'", t);
            if (r == null && __l == 0 && context is Qontext) r = (context as Qontext).lookupOrThrow(key.ToString());

            t = peek();
            if (t.check(Struqture.MEMBER)) {
                Reference nc = Native.get(r.getTrueValue().type, peek(1).str());
                if (nc == null) {
                    if (r?.getTrueValue() is Qontext) {
                        digest(); // <~ into the void!
                        return rrra((Qontext)r.getTrueValue(), ++__l, __buffer);
                    } else if (r?.getTrueValue() is Array) {
                        digest();
                        return rrra((Array)r.getTrueValue(), ++__l, __buffer);
                    }
                } else {
                    if (nc != null) {
                        digest();
                        digest();
                        __buffer.Add(nc);
                        return __buffer.ToArray();
                    }
                }
                throw new ParseException("tried to access member of a value that is not a context and thus memberless,\nor tried to call unknown native funqtion.", t);
            } else {
                if (r == null && key != null) {
                    Log.spam("rrr detected undeclared identifier at the end of the member chain,\nreturning floating reference for possible binding...");
                    r = new FloatingReference<T>(key, context);
                    __buffer.Add(r);
                }
                return __buffer.ToArray();
            }
        }
        
        public Reference rrr(Qontext context, int item = 0) {
            Reference[] r = rrra(context);
            if (r.Length > item) return r[r.Length - (item + 1)];
            else return null;
        }

        public Value readNextValue(Qontext context, ValueType expected = ValueType.Any) {
            Log.spam("Interpretoken.readNextValue()");
            Token t = peek();
            Value result = null;
            if ((t.check(ValueType.Identifier)) ||
                    (t.check(ValueType.Keyword) &&
                    (t.check(Keyword.CURRENT_CONTEXT.name) ||
                    t.check(Keyword.PARENT_CONTEXT.name)))) {
                Log.spam("detected possible reference / identifier");
                Reference[] _r = rrra(context);
                Reference r = _r[_r.Length - 1];
                if (peek().check(Struqture.Call[OPEN])) {
                    Log.spam("hey, it's a funqtion call!");
                    if (r.getValueType() == ValueType.Funqtion || r.getValueType() == ValueType.NativeCall) {
                        Value[] p = Funqtionizer.parseParameters(context, readBody(true));
                        result = (r.getTrueValue() as Funqtion).execute(p, (_r.Length > 1 ? _r[_r.Length - 2] : null));
                    } else throw new ParseException("can not call " + peek(-1) + ": not a funqtion.");
                } else {
                    result = r;
                }
            } else if (t.check(ValueType.Struqture)) {
                Log.spam("detected possible structure");
                if (t.check(Struqture.Funqtion[OPEN])) {
                    Log.spam("...it's a funqtion");
                    Funqtion f = Funqtionizer.parse(context, readBody(true));
                    if (peek().check(Struqture.Call[OPEN])) {
                        Value[] p = Funqtionizer.parseParameters(context, readBody(true));
                        result = f.execute(p);
                    } else {
                        result = f;
                    }
                } else if (t.check(Struqture.Context[OPEN])) {
                    Obqect o = Colleqtionizer.readObqect(context, readBody(true));
                    result = o;
                } else if (t.check(Struqture.Collection[OPEN])) {
                    Array a = Colleqtionizer.readArray(context, readBody(true));
                    result = a;
                } else if (t.check(Struqture.Call[OPEN])) {
                    Segment s = Segmentizer.parseOne(context, readBody());
                    result = s.execute(context);
                }
            } else if (t.check(ValueType.Primitive)) {
                result = digest().makeValue();
            } else if (t.check(ValueType.Operator)) {
                throw new ParseException("next token was unreadable as value: " + t, t);
            }

            return result;
        }
    }
}
