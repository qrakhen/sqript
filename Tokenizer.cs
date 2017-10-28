using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Token : Value
    {
        private Token(Type type, object value) : base(type, value) { }

        public override void setValue<T>(T value) {
            throw new Exception("token value is read only");
        }

        public static Token create(Type type, string value) {
            object parsed = null;
            switch (type) {
                case Type.KEYWORD: parsed = Keywords.get(value); break;
                case Type.OPERATOR: parsed = Operators.get(value); break;
                case Type.INTEGER: parsed = Int32.Parse(value); break;
                case Type.DECIMAL: parsed = Decimal.Parse(value); break;
                case Type.BOOLEAN: parsed = Boolean.Parse(value); break;
                default: parsed = value; break;
            }
            return new Token(type, parsed);
        }
    }

    public class Tokenizer
    {
        private List<Token> stack;
        private string sequence;
        private long position;

        public Tokenizer(string sequence) {
            this.sequence = sequence;
        }

        private string peek() {
            if (position >= sequence.Length) return null;
            return sequence.Substring((int) position, 1);
        }

        private string digest() {
            return sequence.Substring((int) position++, 1);
        }

        public Token[] parse() {
            if (sequence.Length < 1) throw new InvalidOperationException("input sequence empty");
            position = 0;
            stack = new List<Token>();

            do {
                string cur = peek();
                if (Is.Whitespace(cur)) digest();
                else if (Is.Structure(cur)) addToken(digest(), Value.Type.STRUCTURE);
                else if (Is.Operator(cur)) addToken(readOperator(), Value.Type.OPERATOR);
                else if (Is.String(cur)) addToken(readString(), Value.Type.STRING);
                else if (Is.Number(cur)) addToken(readNumber(), Value.Type.NUMBER);
                else if (Is.Identifier(cur)) addToken(readIdentifier(), Value.Type.IDENTIFIER);
                else throw new Exception("unreadable symbol " + cur);
            } while (peek() != null);

            return stack.ToArray();
        }

        private void addToken(string value, Value.Type type) {
            if (type != Value.Type.STRING && Keywords.get(value) != null) type = Value.Type.KEYWORD;
            else if (type != Value.Type.STRING && Operators.get(value) != null) type = Value.Type.OPERATOR;
            else if (type == Value.Type.NUMBER) type = (value.IndexOf(".") < 0 ? Value.Type.INTEGER : Value.Type.DECIMAL);
            stack.Add(Token.create(type, value));
            Console.Write("{ " + type.ToString() + " [ " + value + " ] }" + (value == ";" || peek() == null ? Environment.NewLine : " >> "));
        }

        private string readOperator() {
            string buffer = "";
            do {
                if (Is.Operator(peek())) buffer += digest();
                else break;
            } while (peek() != null);
            return buffer;
        }

        private string readNumber() {
            string buffer = "";
            do {
                if (peek() == "." && buffer.IndexOf(".") > -1) break;
                else if (Is.Number(peek())) buffer += digest();
                else break;
            } while (peek() != null);
            return buffer;
        }

        private string readIdentifier() {
            string buffer = "";
            do {
                if (Is.Identifier(peek()) || Is.Number(peek())) buffer += digest();
                else break;
            } while (peek() != null);
            return buffer;
        }

        private string readString() {
            string buffer = "", limiter = digest();
            do {
                if (peek() != limiter) buffer += digest();
                else if (buffer.Substring(buffer.Length - 1, 1) == @"\") buffer = buffer.Substring(0, buffer.Length - 1) + digest();
                else break;
            } while (peek() != null);
            digest();
            return buffer;
        }

        static class Is
        {
            public static bool Operator(string c) {
                return Regex.IsMatch(c, @"[\/\-\*~+=&<>]");
            }

            public static bool Structure(string c) {
                return Regex.IsMatch(c, @"[{}()[\].,:;]");
            }

            public static bool String(string c) {
                return Regex.IsMatch(c, "[\"']");
            }

            public static bool Number(string c) {
                return Regex.IsMatch(c, @"[\d.]");
            }

            public static bool Whitespace(string c) {
                return Regex.IsMatch(c, @"\s");
            }

            public static bool Identifier(string c) {
                return (!Operator(c) && !Structure(c) && !String(c) && !Number(c) && !Whitespace(c));
            }
        }
    }
}
