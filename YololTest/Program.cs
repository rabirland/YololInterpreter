using System;
using System.Collections.Generic;

namespace YololTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Tokenizer tokenizer = new Tokenizer();
			Lexer lexer = new Lexer();
			Interpreter interpreter = new Interpreter();

			var fields = new Dictionary<string, YololValue>();
			var externalFields = new Dictionary<string, YololValue>();

			while (true)
			{
				Console.Write("Expression: ");
				var line = Console.ReadLine();
				if (line.StartsWith("set"))
				{
					string[] parms = line.Split(' ');
					YololValue val = decimal.TryParse(parms[2], out decimal numVal) ? new YololValue(numVal) : new YololValue(parms[2]);
					if (parms[1][0] == ':')
					{
						externalFields[parms[1]] = val;
					}
					else
					{
						fields[parms[1]] = val;
					}
				}
				else if (line == "get")
				{
					foreach (var entry in fields)
					{
						Console.WriteLine($"{entry.Key} = {entry.Value}");
					}

					foreach (var entry in externalFields)
					{
						Console.WriteLine($"{entry.Key} = {entry.Value}");
					}
				}
				else
				{
					try
					{
						var tokens = tokenizer.ParseLine(line);
						var lexes = lexer.TranslateLine(tokens);
						interpreter.InterpretLine(lexes, fields, externalFields);
					}
					catch (Exception e)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(e.Message);
						Console.ForegroundColor = ConsoleColor.Gray;
					}
				}
			}

			Console.WriteLine("-= DONE =-");
			Console.ReadLine();
		}
	}
}
