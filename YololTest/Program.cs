using System;
using Yolol.Grammar;

namespace YololTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Lexer lexer = new Lexer();
			var lexes = lexer.Parse("b=3.324234242342432");

			foreach (var lex in lexes)
            {
				Console.WriteLine(lex);
            }

			Console.ReadLine();
		}
	}
}
