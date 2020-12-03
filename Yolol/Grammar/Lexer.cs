using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Yolol.Grammar
{
    public class Lexer
    {
        private static readonly Regex numericLiteralRegex = new Regex(@"[0-9]+(\.[0-9]+)?", RegexOptions.Compiled);
        private static readonly Regex stringLiteralRegex = new Regex(@"\""(\\\""|.)*?\""", RegexOptions.Compiled);
        private static readonly Regex identifierRegex = new Regex(@"\:?[a-zA-Z]+", RegexOptions.Compiled);

        private static readonly string[] textOperators = new[] { "ABS", "SQRT", "SIN", "COS", "TAN", "ASIN", "ACOS", "ATAN", "NOT", "AND", "OR" };
        private static readonly string[] commands = new[] { "GOTO" };
        private static readonly string[] keywords = new[] { "IF", "THEN", "ELSE", "END" };
        // Must be DESC ordered by length
        private static readonly string[] validOperators = new[] { "++", "--", "+=", "-=", "*=", "/=", "%=", "=", "+", "-", "*", "/", "%", "^", "!", };
        private static readonly string[] afterIdentifierOperators = new[] { "++", "--" };
        private static readonly string[] beforeIdentifierOperators = new[] { "++", "--" };

        public IEnumerable<Lex> Parse(string line)
        {
            line = line.ToUpperInvariant();
            List<Lex> ret = new List<Lex>();
            LexType? previousLexType = null;
            int pos = 0;

            while (pos < line.Length)
            {
                // Skip whitespaces
                while (pos < line.Length && char.IsWhiteSpace(line[pos]))
                {
                    pos++;
                }

                var lex = TryFetchLexAtPosition(line, pos, previousLexType);
                if (lex != null)
                {
                    ret.Add(lex.Value);
                    previousLexType = lex.Value.Type;
                    pos += lex.Value.Length;
                }
                else if (pos < line.Length)
                {
                    throw new System.Exception("Unparsed lexes");
                }
            }

            return ret;
        }

        private Lex? TryFetchLexAtPosition(string line, int pos, LexType? previousLexType)
        {
            if (pos < line.Length)
            {
                // Parse numeric literals
                var numberLiteral = TryFetchNumericLiteral(line, pos);
                if (numberLiteral != null)
                {
                    var digit = numberLiteral.IndexOf('.');
                    var stripped = digit > 0
                        ? numberLiteral.Substring(0, digit) + '.' + numberLiteral.Substring(digit + 1, Math.Min(3, numberLiteral.Length - digit - 1))
                        : numberLiteral;

                    return new Lex() { Part = stripped, Start = pos, Length = numberLiteral.Length, Type = LexType.NumericLiteral };
                }

                // Parse string literals
                var stringLiteral = TryFetchStringLiteral(line, pos);
                if (stringLiteral != null)
                {
                    return new Lex() { Part = stringLiteral, Start = pos, Length = stringLiteral.Length, Type = LexType.StringLiteral };
                }

                // Parse symbol operators
                var operatorSymbol = TryFetchOperator(line, pos, previousLexType);
                if (operatorSymbol != null)
                {
                    return new Lex() { Part = operatorSymbol, Start = pos, Length = operatorSymbol.Length, Type = LexType.Operator };
                }

                // Parse identifiers
                var identifier = TryFetchIdentifier(line, pos);
                if (identifier != null)
                {
                    if (textOperators.Contains(identifier))
                    {
                        return new Lex() { Part = identifier, Start = pos, Length = identifier.Length, Type = LexType.Operator };
                    }
                    else if (commands.Contains(identifier))
                    {
                        return new Lex() { Part = identifier, Start = pos, Length = identifier.Length, Type = LexType.Command };
                    }
                    else if (keywords.Contains(identifier))
                    {
                        return new Lex() { Part = identifier, Start = pos, Length = identifier.Length, Type = LexType.Keyword };
                    }
                    else
                    {
                        return new Lex() { Part = identifier, Start = pos, Length = identifier.Length, Type = LexType.Identifier };
                    }
                }

                if (line[pos] == '(')
                {
                    return new Lex() { Part = "(", Start = pos, Length = 1, Type = LexType.OpenBracket };
                }

                if (line[pos] == ')')
                {
                    return new Lex() { Part = ")", Start = pos, Length = 1, Type = LexType.CloseBracket };
                }

                throw new System.Exception("Something really bad happened :(");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to fetch a numeric literal from the given <paramref name="input"/> at <paramref name="pos"/> position.
        /// </summary>
        /// <returns>The string representation or <see langword="null"/> if couldn't fetch.</returns>
        private static string TryFetchNumericLiteral(string line, int pos)
        {
            var match = numericLiteralRegex.Match(line, pos);
            if (match.Success && match.Index == pos)
            {
                return match.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to fetch a string literal from the given <paramref name="input"/> at <paramref name="pos"/> position.
        /// </summary>
        /// <returns>The string representation or <see langword="null"/> if couldn't fetch.</returns>
        private static string TryFetchStringLiteral(string line, int pos)
        {
            var match = stringLiteralRegex.Match(line, pos);
            if (match.Success && match.Index == pos)
            {
                return match.Value.Substring(1, match.Value.Length - 2);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to fetch an identifier from the given <paramref name="input"/> at <paramref name="pos"/> position.
        /// </summary>
        /// <returns>The string representation or <see langword="null"/> if couldn't fetch.</returns>
        private static string TryFetchIdentifier(string line, int pos)
        {
            var match = identifierRegex.Match(line, pos);
            if (match.Success && match.Index == pos)
            {
                return match.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Tries to fetch an operator
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pos"></param>
        /// <param name="previousLexType"></param>
        /// <returns>The string representation or <see langword="null"/> if couldn't fetch any.</returns>
        private static string TryFetchOperator(string line, int pos, LexType? previousLexType)
        {
            var remain = line[pos..];
            for (int i = 0; i < validOperators.Length; i++)
            {
                var op = validOperators[i];
                if (remain.StartsWith(op))
                {
                    var isafterOperator = afterIdentifierOperators.Contains(op);
                    var isBeforeOperator = beforeIdentifierOperators.Contains(op);
                    var isAttachedOperator = isafterOperator || isBeforeOperator;

                    // If the operator must be before/after an identifier
                    if (isAttachedOperator)
                    {
                        if (isafterOperator && previousLexType == LexType.Identifier)
                        {
                            return op;
                        }
                        else if (isBeforeOperator && TryFetchIdentifier(line, pos + op.Length) != null)
                        {
                            return op;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        return op;
                    }
                }
            }

            return null;
        }
    }
}
