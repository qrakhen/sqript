using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Qrakhen.Sqript {

	public class Token : QValue {

		public readonly int line, col;

		private Token(ValueType type, object value, int line = -1, int col = -1) : base(value, type) {
			this.line = line;
			this.col = col;
		}

		public override void SetValue(object value, ValueType type) {
			throw new Exception("token value is read only", this);
		}

		public bool Check(ValueType type, object value) {
			return (this.Type == type && this.Value == value);
		}

		public bool Check(string value) {
			return (this.Value?.ToString() == value);
		}

		public bool Check(ValueType type) {
			return IsType(type);
		}

		public bool Check(Keyword.Kwrd kwrd) {
			return (Value?.ToString() == Keywords.Get(kwrd.name).ToString());
		}

		public QValue MakeValue() {
			return new QValue(Value, Type);
		}

		public static Token Create(ValueType type, string value, int line = -1, int col = -1) {
			object parsed;
			NumberFormatInfo format = new NumberFormatInfo {
				NumberDecimalSeparator = "."
			};
			parsed = type switch {
				ValueType.Keyword => Keywords.Get(value),
				ValueType.Operator => Operators.Get(value),
				ValueType.Integer => Int32.Parse(value),
				ValueType.Decimal => System.Decimal.Parse(value, format),
				ValueType.Boolean => Boolean.Parse(value),
				_ => value,
			};
			return new Token(type, parsed, line, col);
		}

		public string GetLocation() {
			return " @ " + line + " : " + col;
		}
	}

	internal class Tokenizer : Digester<char> {
		private List<Token> _result;
		private int __line = 0, __col = 0;

		public Tokenizer(string sequence) : this(sequence.ToCharArray()) { }

		private Tokenizer(char[] stack) : base(stack) { }

		protected new string Peek() {
			return base.Peek().ToString();
		}

		protected new string Peek(int shift) {
			return base.Peek(shift).ToString();
		}

		protected new string Digest() {
			__col++;
			return base.Digest().ToString();
		}

		public Token[] Parse() {
			if (stack.Length < 1)
				throw new InvalidOperationException("input sequence empty");
			_result = new List<Token>();

			do {
				string cur = Peek();
				if (Is.Whitespace(cur)) {
					if (Digest() == "\n") {
						__line++;
						__col = 0;
					}
				} else if (Is.Comment(cur)) {
					Digest();
					string close = "\n";
					if (Peek() == "/")
						close = "/#";
					string start = Digest();
					do {
						start = Digest();
						if (close == "/#" && start == "/" && Peek() == "#") {
							Peek();
							Digest();
							break;
						} else if (start == close) {
							break;
						}
					} while (!EndOfStack());
				} else if (Is.Structure(cur))
					AddToken(ReadStructure(), ValueType.Struqture, __line, __col);
				else if (Is.Operator(cur))
					AddToken(ReadOperator(), ValueType.Operator, __line, __col);
				else if (Is.String(cur))
					AddToken(ReadString(), ValueType.String, __line, __col);
				else if (Is.Number(cur))
					AddToken(ReadNumber(), ValueType.Number, __line, __col);
				else if (Is.Identifier(cur)) {
					string identifier = ReadIdentifier();
					if (identifier.Equals("true") || identifier.Equals("false")) {
						AddToken(identifier, ValueType.Boolean, __line, __col);
					} else {
						AddToken(identifier, ValueType.Identifier, __line, __col);
					}
				} else {
					throw new Exception("unreadable symbol " + cur);
				}
			} while (!EndOfStack());
			return _result.ToArray();
		}

		private void AddToken(string value, ValueType type, int line = 0, int col = 0) {
			if (type == ValueType.Boolean)
				type = ValueType.Boolean;
			else if (type != ValueType.String && Keywords.Get(value) != null)
				type = ValueType.Keyword;
			else if (type != ValueType.String && Operators.Get(value) != null)
				type = ValueType.Operator;
			else if (type == ValueType.Number)
				type = (value.IndexOf(".") < 0 ? ValueType.Integer : ValueType.Decimal);
			_result.Add(Token.Create(type, value, line, col));
			Log.Spam("[" + type.ToString() + "] > '" + value + "'");
		}

		private static readonly string[] SQR_SPECIAL = new string[] {
			"~(", ".~", ":~", "*~", "*:", "^~", "?~", "~?"
		};

		// ToDo: Types
		// private string TYPED_REF = "*type~";

		private string ReadStructure() {
			string buffer = Digest();
			do {
				string next = Peek();
				foreach (string s in SQR_SPECIAL) {
					if (buffer + next == s) {
						buffer += Digest();
						break;
					}
				}
				break;
			} while (!EndOfStack());
			return buffer;
		}

		private string ReadOperator() {
			string buffer = "";
			do {
				if (Is.Operator(Peek()))
					buffer += Digest();
				else if (Is.Structure(Peek())) {
					string next = Peek();
					foreach (string s in SQR_SPECIAL) {
						if (buffer + next == s) {
							buffer += Digest();
							break;
						}
					}
					break;
				} else {
					break;
				}
			} while (!EndOfStack());
			return buffer;
		}

		private string ReadNumber() {
			string buffer = "";
			do {
				if (Peek() == "." && buffer.IndexOf(".") > -1)
					break;
				else if (Is.Number(Peek())) {
					buffer += Digest();
				} else {
					break;
				}
			} while (!EndOfStack());
			return buffer;
		}

		private string ReadIdentifier() {
			string buffer = "";
			do {
				if (Is.Identifier(Peek()) || Is.Number(Peek())) {
					buffer += Digest();
					/*} else if (Peek() == Qontext.MEMBER_DELIMITER
					  && buffer.Length > 0
					  && buffer.Substring(buffer.Length - 1) != Qontext.MEMBER_DELIMITER) {
						if (Is.Identifier(Peek(1)))
							buffer += Digest();
					*/
				} else {
					break;
				}
			} while (!EndOfStack());
			return buffer;
		}

		private string ReadString() {
			string buffer = "", limiter = Digest();
			do {
				if (Peek() != limiter)
					buffer += Digest();
				else if (buffer.Length != 0 && buffer.Substring(buffer.Length - 1, 1) == @"\")
					buffer = buffer[0..^1] + Digest();
				else { Digest(); break; }
			} while (!EndOfStack());
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
