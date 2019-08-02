using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace YololTest
{
	class Lexer
	{
		private static readonly string[] functions = { "sqrt", "sin", "cos", "tan", "arcsin", "arccos", "arctan" };
		private static readonly string[] keywords = { "if", "else", "end", "goto" };
		private static readonly string[] unaryOperators = { "++", "--", "!" };
		private static readonly string[] dualOperators = { "+", "-", "*", "/", "%", "^", "=", "+=", "-=", "*=", "/=", /*Logic: */ "<", ">", "<=", ">=", "!=", "==" };

		public Lex[] TranslateLine(Token[] tokens)
		{
			List<Lex> ret = new List<Lex>();

			foreach (var token in tokens)
			{
				switch (token.Type)
				{
					case TokenType.Operator:
						if (unaryOperators.Contains(token.Value))
						{
							ret.Add(new Lex(token.Value, LexType.UnaryOperator));
						}
						else if (dualOperators.Contains(token.Value))
						{
							ret.Add(new Lex(token.Value, LexType.DualOperator));
						}
						else
						{
							throw new Exception("Unknown operator");
						}
						break;
					case TokenType.Number:
						int dot = token.Value.IndexOf('.');
						if (dot < 0 || token.Value.Length - dot < 4)
						{
							if (decimal.TryParse(token.Value, out decimal d))
							{
								ret.Add(new Lex(token.Value, LexType.Number));
							}
							else
							{
								throw new Exception("Invalid number");
							}
						}
						else
						{
							throw new Exception("Too many digit");
						}
						break;
					case TokenType.String:
						ret.Add(new Lex(token.Value.Substring(1, token.Value.Length - 2), LexType.String)); // Remove " characters
						break;
					case TokenType.Identifier:
						if (keywords.Contains(token.Value))
						{
							ret.Add(new Lex(token.Value, LexType.Keyword));
						}
						else if (functions.Contains(token.Value))
						{
							ret.Add(new Lex(token.Value, LexType.Function));
						}
						else if (token.Value[0] == ':')
						{
							ret.Add(new Lex(token.Value, LexType.ExternalIdentifier));
						}
						else
						{
							ret.Add(new Lex(token.Value, LexType.Identifier));
						}
						break;
					case TokenType.Unknown: throw new Exception("Unknown token type");
				}
			}

			return ret.ToArray();
		}
	}

	public class Lex
	{
		public string Value { get; }

		public LexType Type { get; }

		public Lex(string value, LexType type)
		{
			this.Value = value;
			this.Type = type;
		}

		public override string ToString() => $"[{this.Type}: {this.Value}]";
	}

	public enum LexType
	{
		Unknown = 0,

		UnaryOperator = 1,
		DualOperator = 2,

		String = 3,
		Number = 4,

		Keyword,
		Function,
		ExternalIdentifier,
		Identifier,
	}
}
