﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript {
	public class Token : Value {
		public readonly int line, col;

		private Token(ValueType type, object value, int line = -1, int col = -1) : base(value, type) {
			this.line = line;
			this.col = col;
		}

		public override void setValue(object value, ValueType type) {
			throw new Exception("token value is read only", this);
		}

		public bool check(ValueType type, object value) {
			return (this.type == type && this.value == value);
		}

		public bool check(string value) {
			return (this.value?.ToString() == value);
		}

		public bool check(ValueType type) {
			return isType(type);
		}

		public bool check(Keyword.Kwrd kwrd) {
			return (value?.ToString() == Keywords.get(kwrd.name).ToString());
		}

		public Value makeValue() {
			return new Value(value, type);
		}

		public static Token create(ValueType type, string value, int line = -1, int col = -1) {
			object parsed = null;
			NumberFormatInfo format = new NumberFormatInfo();
			format.NumberDecimalSeparator = ".";
			switch(type) {
				case ValueType.Keyword:
					parsed = Keywords.get(value);
					break;
				case ValueType.Operator:
					parsed = Operators.get(value);
					break;
				case ValueType.Integer:
					parsed = Int32.Parse(value);
					break;
				case ValueType.Decimal:
					parsed = System.Decimal.Parse(value, format);
					break;
				case ValueType.Boolean:
					parsed = Boolean.Parse(value);
					break;
				default:
					parsed = value;
					break;
			}
			return new Token(type, parsed, line, col);
		}

		public string getLocation() {
			return " @ " + line + " : " + col;
		}
	}

	internal class Tokenizer : Digester<char> {
		private List<Token> result;
		private int __line = 0, __col = 0;

		public Tokenizer(string sequence) : this(sequence.ToCharArray()) { }

		private Tokenizer(char[] stack) : base(stack) { }

		protected new string peek() {
			return base.peek().ToString();
		}

		protected new string peek(int shift) {
			return base.peek(shift).ToString();
		}

		protected new string digest() {
			__col++;
			return base.digest().ToString();
		}

		public Token[] parse() {
			if(stack.Length < 1)
				throw new InvalidOperationException("input sequence empty");
			result = new List<Token>();

			do {
				string cur = peek();
				if(Is.Whitespace(cur)) {
					if(digest() == "\n") {
						__line++;
						__col = 0;
					}
				} else if(Is.Comment(cur)) {
					string start = digest();
					string close = "\n";
					if(peek() == "/")
						close = "/#";
					string s = digest();
					do {
						s = digest();
						if(close == "/#" && s == "/" && peek() == "#") {
							peek();
							digest();
							break;
						} else if(s == close) {
							break;
						}
					} while(!endOfStack());
				} else if(Is.Structure(cur))
					addToken(readStructure(), ValueType.Struqture, __line, __col);
				else if(Is.Operator(cur))
					addToken(readOperator(), ValueType.Operator, __line, __col);
				else if(Is.String(cur))
					addToken(readString(), ValueType.String, __line, __col);
				else if(Is.Number(cur))
					addToken(readNumber(), ValueType.Number, __line, __col);
				else if(Is.Identifier(cur)) {
					string identifier = readIdentifier();
					if(identifier.Equals("true") || identifier.Equals("false")) {
						addToken(identifier, ValueType.Boolean, __line, __col);
					} else {
						addToken(identifier, ValueType.Identifier, __line, __col);
					}
				}
				else
					throw new Exception("unreadable symbol " + cur);
			} while(!endOfStack());
			return result.ToArray();
		}

		private void addToken(string value, ValueType type, int line = 0, int col = 0) {
			if(type == ValueType.Boolean)
				type = ValueType.Boolean;
			else if(type != ValueType.String && Keywords.get(value) != null)
				type = ValueType.Keyword;
			else if(type != ValueType.String && Operators.get(value) != null)
				type = ValueType.Operator;
			else if(type == ValueType.Number)
				type = (value.IndexOf(".") < 0 ? ValueType.Integer : ValueType.Decimal);
			result.Add(Token.create(type, value, line, col));
			Log.spam("[" + type.ToString() + "] > '" + value + "'");
		}

		private string[] SQR_SPECIAL = new string[] {
			"~(", ".~", ":~", "*~", "*:", "^~", "?~", "~?"
		};

		// ToDo: Types
		private string TYPED_REF = "*type~";

		private string readStructure() {
			string buffer = digest();
			do {
				string next = peek();
				foreach(string s in SQR_SPECIAL) {
					if(buffer + next == s) {
						buffer += digest();
						break;
					}
				}
				break;
			} while(!endOfStack());
			return buffer;
		}

		private string readOperator() {
			string buffer = "";
			do {
				if(Is.Operator(peek()))
					buffer += digest();
				else if(Is.Structure(peek())) {
					string next = peek();
					foreach(string s in SQR_SPECIAL) {
						if(buffer + next == s) {
							buffer += digest();
							break;
						}
					}
					break;
				} else
					break;
			} while(!endOfStack());
			return buffer;
		}

		private string readNumber() {
			string buffer = "";
			do {
				if(peek() == "." && buffer.IndexOf(".") > -1)
					break;
				else if(Is.Number(peek()))
					buffer += digest();
				else
					break;
			} while(!endOfStack());
			return buffer;
		}

		private string readBool() {
			string buffer = "";
			do {
				if(peek() == "." && buffer.IndexOf(".") > -1)
					break;
				else if(Is.Number(peek()))
					buffer += digest();
				else
					break;
			} while(!endOfStack());
			return buffer;
		}

		private string readIdentifier() {
			string buffer = "";
			do {
				if(Is.Identifier(peek()) || Is.Number(peek()))
					buffer += digest();
				/*else if (peek() == Context.MEMBER_DELIMITER
						&& buffer.Length > 0
						&& buffer.Substring(buffer.Length - 1) != Context.MEMBER_DELIMITER) {
					if (Is.Identifier(peek(1))) buffer += digest();
					else break; }*/
				else
					break;
			} while(!endOfStack());
			return buffer;
		}

		private string readString() {
			string buffer = "", limiter = digest();
			do {
				if(peek() != limiter)
					buffer += digest();
				else if(buffer.Length != 0 && buffer.Substring(buffer.Length - 1, 1) == @"\")
					buffer = buffer.Substring(0, buffer.Length - 1) + digest();
				else { digest(); break; }
			} while(!endOfStack());
			return buffer;
		}

		static class Is {
			public static bool Operator(string c) {
				return Regex.IsMatch(c, @"[\/\-\*+=&<>~^?!]");
			}

			public static bool Structure(string c) {
				return Regex.IsMatch(c, @"[{}()[\].,:;~]");
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

			public static bool Comment(string c) {
				return Regex.IsMatch(c, @"#");
			}

			public static bool Identifier(string c) {
				return (!Operator(c) && !Structure(c) && !String(c) && !Number(c) && !Whitespace(c));
			}
		}
	}
}
