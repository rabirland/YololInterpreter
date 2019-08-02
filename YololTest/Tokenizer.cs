using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YololTest
{
	class Tokenizer
	{
		private static readonly Regex operatorsRegex = new Regex(@"\G[+\-*/<>=!%\^]+", RegexOptions.Compiled);
		private static readonly Regex stringRegex = new Regex(@"\G\"".*?\""", RegexOptions.Compiled);
		private static readonly Regex numberRegex = new Regex(@"\G\d+(\.\d+)?[a-z]*", RegexOptions.Compiled);
		private static readonly Regex identifierRegex = new Regex(@"\G\:?(?:[a-z][a-z0-9]*)", RegexOptions.Compiled);
		private static readonly Regex parenthesisRegex = new Regex(@"\G(\(|\))", RegexOptions.Compiled);
		private static readonly string commentPrefix = "//";

		public Token[] ParseLine(string line)
		{
			List<Token> ret = new List<Token>();
			// Strip comments
			var commentStart = line.IndexOf(commentPrefix);
			if (commentStart > 0)
			{
				line = line.Substring(0, commentStart);
			}
			else if (commentStart == 0)
			{
				return new Token[0];
			}

			int index = 0;
			while (index < line.Length)
			{
				// Skip the whitespaces, if there was only whitespace => we finished lexing the line
				int nextIndex = line.IndexOfNot(' ', index);
				if (nextIndex >= index)
				{
					index = nextIndex;
				}
				else
				{
					break;
				}

				if (TryMatch(line, index, operatorsRegex, out string value))
				{
					ret.Add(new Token(value, TokenType.Operator));
				}
				else if (TryMatch(line, index, numberRegex, out value))
				{
					ret.Add(new Token(value, TokenType.Number));
				}
				else if (TryMatch(line, index, stringRegex, out value))
				{
					ret.Add(new Token(value, TokenType.String));
				}
				else if (TryMatch(line, index, identifierRegex, out value))
				{
					ret.Add(new Token(value, TokenType.Identifier));
				}
				else if (TryMatch(line, index, parenthesisRegex, out value))
				{
					ret.Add(new Token(value, TokenType.Parenthesis));
				}
				else
				{
					throw new Exception($"Uncrecognized code at index {index}");
				}

				index += value?.Length ?? 0;
			}

			return ret.ToArray();
		}

		private bool TryMatch(string line, int index, Regex regex, out string value)
		{
			var match = regex.Match(line, index);
			if (match.Success && match.Index == index)
			{
				value = match.Value;
				return true;
			}
			else
			{
				value = null;
				return false;
			}
		}
	}

	static class StringExtension
	{
		public static int IndexOfNot(this string str, char c, int startIndex)
		{
			for (int i = startIndex; i < str.Length; i++)
			{
				if (str[i] != c) return i;
			}

			return -1;
		}
	}

	public class Token
	{
		public string Value { get; }

		public TokenType Type { get; }

		public Token(string value, TokenType type)
		{
			this.Value = value;
			this.Type = type;
		}

		public override string ToString() => $"[{this.Type}: {this.Value}]";
	}

	public enum TokenType
	{
		Unknown = 0,
		Operator = 1,
		Identifier = 2,
		String = 3,
		Number = 4,
		Parenthesis = 5,
	}
}
