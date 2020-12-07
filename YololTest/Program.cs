using System;
using Yolol.Grammar;

namespace YololTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Lexer lexer = new Lexer();
			var lexes = lexer.Parse("if bela>2AND3<sanyi then b=2else c=2end");

			foreach (var lex in lexes)
            {
				Console.WriteLine(lex);
            }

			Console.ReadLine();
		}
	}
}
