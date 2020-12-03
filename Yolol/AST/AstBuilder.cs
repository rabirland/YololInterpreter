using System.Collections.Generic;
using System.Linq;
using Yolol.AST.Nodes;
using Yolol.Grammar;

namespace Yolol.AST
{
    public class AstBuilder
    {
        public void Parse(IEnumerable<Lex> lexes)
        {
            var lexList = lexes.ToArray();
            var ret = new List<INode>();

            INode currentNode = null;

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
            else if (currentLex.Type == LexType.Command)
            {
                if (currentLex.Part == "GOTO")
                {
                    return new CommandNode() { Type = CommandType.Goto };
                }
                else
                {
                    throw new System.Exception("Unsupported command");
                }
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

            return null;
        }

        /// <summary>
        /// Counts the amount of lex between the brackets, including nesting.
        /// </summary>
        /// <param name=""></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        private int CountParenthesis(Lex[] lexes, int startPos)
        {
            return 0;
        }
    }
}
