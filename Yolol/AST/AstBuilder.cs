using System.Collections.Generic;
using System.Linq;
using Yolol.AST.Nodes;
using Yolol.Grammar;

namespace Yolol.AST
{
    public class AstBuilder
    {
        public IEnumerable<INode> Parse(IEnumerable<Lex> lexes)
        {
            var lexList = lexes.ToArray();
            var ret = new List<INode>();

            return ret;
        }

        private INode FetchNextNode(Lex[] lexes, int pos)
        {
            var currentLex = lexes[pos];
            if (currentLex.Type == LexType.NumericLiteral)
            {
                return new LiteralNode() { Value = currentLex.Part, Type = LiteralType.Numeric };
            }
            else if (currentLex.Type == LexType.StringLiteral)
            {
                return new LiteralNode() { Value = currentLex.Part, Type = LiteralType.String };
            }
            else if (currentLex.Type == LexType.Identifier)
            {
                if (currentLex.Part.StartsWith(':'))
                {
                    return new GlobalVariableNode() { Name = currentLex.Part.Substring(1) };
                }
                else
                {
                    return new LocalVariableNode() { Name = currentLex.Part };
                }
            }
            else if (currentLex.Type == LexType.OpenBracket)
			{
                int count = CountParenthesis(lexes, pos + 1);
                var lexesInBracket = lexes[(pos + 1)..(pos + 1 + count)];
                return new BracketValueNode();
			}
            else if (currentLex.Type == LexType.Keyword)
            {
                if (currentLex.Part == "GOTO")
                {
                    return new CommandNode() { Type = CommandType.Goto };
                }
                else if (currentLex.Part == "IF")
				{
                    return new IfThenElseNode();
                }
                else
                {
                    throw new System.Exception("Unknown keyword");
                }
            }

            throw new System.Exception("Unknown lex series");
        }

        /// <summary>
        /// Counts the amount of lex between the brackets, including nesting.
        /// </summary>
        /// <param name="lexes">The list of lexes.</param>
        /// <param name="startPos">The index of the lex after the open bracket.</param>
        /// <returns></returns>
        private static int CountParenthesis(Lex[] lexes, int startPos)
        {
            int level = 0;
            for (int i = startPos; i < lexes.Length; i++)
			{
                if (lexes[i].Type == LexType.OpenBracket)
				{
                    level++;
				}
                else if (lexes[i].Type == LexType.CloseBracket)
				{
                    if (level > 0)
					{
                        level--;
					}
					else
					{
                        return i - startPos;
					}
				}
			}

            throw new System.Exception("No end of parenthesis");
        }

        /// <summary>
        /// Counts the amount of lex between the "if" and "end", including nesting.
        /// </summary>
        /// <param name="lexes">The list of lexes.</param>
        /// <param name="startPos">The index of the lex after the "if".</param>
        /// <returns></returns>
        private static int CountIf(Lex[] lexes, int startPos)
        {
            int level = 0;
            for (int i = startPos; i < lexes.Length; i++)
            {
                if (lexes[i].Type == LexType.Keyword && lexes[i].Part == "IF")
                {
                    level++;
                }
                else if (lexes[i].Type == LexType.Keyword && lexes[i].Part == "END")
                {
                    if (level > 0)
                    {
                        level--;
                    }
                    else
                    {
                        return i - startPos;
                    }
                }
            }

            throw new System.Exception("No end of if-end");
        }
    }
}
