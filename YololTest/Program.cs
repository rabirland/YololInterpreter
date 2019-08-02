using System;

namespace YololTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Tokenizer tokenizer = new Tokenizer();
			Lexer lexer = new Lexer();

			string[] lines =
			{
				"a+b",
			};
			
			foreach (var line in lines)
			{
				var tokens = tokenizer.ParseLine(line);
				var lexes = lexer.TranslateLine(tokens);
			}

			Console.ReadLine();
		}
	}
}
