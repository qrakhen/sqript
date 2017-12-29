﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript
{
    public class Token : Value
    {
        public readonly int line, col;

        private Token(ValueType type, object value, int line = -1, int col = -1) : base(value, type) {
            this.line = line;
            this.col = col;
        }

        public override void setValue(object value, ValueType type) {
            throw new Exception("token value is read only", this);
        }

        public static Token create(ValueType type, string value, int line = -1, int col = -1) {
            object parsed = null;
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";
            switch (type) {
                case ValueType.KEYWORD: parsed = Keywords.get(value); break;
                case ValueType.OPERATOR: parsed = Operators.get(value); break;
                case ValueType.INTEGER: parsed = Int32.Parse(value); break;
                case ValueType.DECIMAL: parsed = Decimal.Parse(value, format); break;
                case ValueType.BOOLEAN: parsed = Boolean.Parse(value); break;
                default: parsed = value; break;
            }
            return new Token(type, parsed, line, col);
        }

        public string getLocation() {
            return " @ " + line + " : " + col;
        }
    }

    public class Tokenizer : Digester<char>
    {
        private List<Token> result;
        private int __line = 0, __col = 0;

        public Tokenizer(string sequence) : this(sequence.ToCharArray()) { }

        private Tokenizer(char[] stack) : base(stack) { }

        protected new string peek() {
            return base.peek().ToString();
        }

        protected new string digest() {
            __col++;
            return base.digest().ToString();
        }

        public Token[] parse() {
            if (stack.Length < 1) throw new InvalidOperationException("input sequence empty");
            result = new List<Token>();

            do {
                string cur = peek();
                if (Is.Whitespace(cur)) {
                    if (digest() == "\n") {
                        __line++;
                        __col = 0;
                    }
                } else if (Is.Structure(cur)) addToken(digest(), ValueType.STRUCTURE);
                else if (Is.Operator(cur)) addToken(readOperator(), ValueType.OPERATOR);
                else if (Is.String(cur)) addToken(readString(), ValueType.STRING);
                else if (Is.Number(cur)) addToken(readNumber(), ValueType.NUMBER);
                else if (Is.Identifier(cur)) addToken(readIdentifier(), ValueType.IDENTIFIER);
                else throw new Exception("unreadable symbol " + cur);
            } while (!endOfStack());
            return result.ToArray();
        }

        private void addToken(string value, ValueType type) {
            if (type != ValueType.STRING && Keywords.get(value) != null) type = ValueType.KEYWORD;
            else if (type != ValueType.STRING && Operators.get(value) != null) type = ValueType.OPERATOR;
            else if (type == ValueType.NUMBER) type = (value.IndexOf(".") < 0 ? ValueType.INTEGER : ValueType.DECIMAL);
            result.Add(Token.create(type, value, __line, __col));
            Debug.spam("{ " + type.ToString() + " [ " + value + " ] }" + (value == ";" || peek() == null ? Environment.NewLine : " >> "));
        }

        private string readOperator() {
            string buffer = "";
            do {
                if (Is.Operator(peek())) buffer += digest();
                else break;
            } while (!endOfStack());
            return buffer;
        }

        private string readNumber() {
            string buffer = "";
            do {
                if (peek() == "." && buffer.IndexOf(".") > -1) break;
                else if (Is.Number(peek())) buffer += digest();
                else break;
            } while (!endOfStack());
            return buffer;
        }

        private string readIdentifier() {
            string buffer = "";
            do {
                if (Is.Identifier(peek()) || Is.Number(peek())) buffer += digest();
                else break;
            } while (!endOfStack());
            return buffer;
        }

        private string readString() {
            string buffer = "", limiter = digest();
            do {
                if (peek() != limiter) buffer += digest();
                else if (buffer.Substring(buffer.Length - 1, 1) == @"\") buffer = buffer.Substring(0, buffer.Length - 1) + digest();
                else { digest(); break; }
            } while (!endOfStack());
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