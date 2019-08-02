using System;
using System.Collections.Generic;
using System.Text;

namespace YololTest
{
	class Compiler
	{
		//public Code[] CompileLine(Lex[] lexes)
		//{
			
		//}

		//private Code[] CompileNext(Lex[] lexes, int index)
		//{
		//	if (lexes[index].Type == LexType.Identifier) // a[operator]b OR a[operator]
		//	{

		//	}
		//}



		//private Code TransformPrimitive(Lex lex)
		//{
		//	switch (lex.Type)
		//	{
		//		case LexType.Number: return new Code(lex.Value, CodeType.Number);
		//		case LexType.String: return new Code(lex.Value, CodeType.String);
		//		case LexType.Identifier: return new Code(lex.Value, CodeType.Access);
		//		case LexType.ExternalIdentifier: return new Code(lex.Value, CodeType.ExternalAccess);
		//		default: return null;
		//	}
		//}

		//private bool MatchPattern(Lex[] lexes, int startIndex, LexType[] pattern)
		//{
		//	if (startIndex + pattern.Length <= lexes.Length)
		//	{
		//		for (int i = 0; i < pattern.Length; i++)
		//		{
		//			if (lexes[startIndex + i].Type != pattern[i]) return false;
		//		}

		//		return true;
		//	}
		//	else
		//	{
		//		return false;
		//	}
		//}
	}

	public abstract class Code
	{
		
	}

	public abstract class ValueCode : Code
	{
		public abstract YololValue Calculate();
	}

	public class AddCode : ValueCode
	{
		public ValueCode Left { get; }
		public ValueCode Right { get; }

		public override YololValue Calculate() => Left.Calculate() + Right.Calculate();
	}

	public class SubtractCode : ValueCode
	{
		public ValueCode Left { get; }
		public ValueCode Right { get; }

		public override YololValue Calculate() => Left.Calculate() - Right.Calculate();
	}

	public class MultiplyCode : ValueCode
	{
		public ValueCode Left { get; }
		public ValueCode Right { get; }

		public override YololValue Calculate() => Left.Calculate() * Right.Calculate();
	}

	public class DivideCode : ValueCode
	{
		public ValueCode Left { get; }
		public ValueCode Right { get; }

		public override YololValue Calculate() => Left.Calculate() / Right.Calculate();
	}

	public class ModuloCode : ValueCode
	{
		public ValueCode Left { get; }
		public ValueCode Right { get; }

		public override YololValue Calculate() => Left.Calculate() % Right.Calculate();
	}

	public class PowerCode : ValueCode
	{
		public ValueCode Left { get; }
		public ValueCode Right { get; }

		public override YololValue Calculate() => Left.Calculate() ^ Right.Calculate();
	}

	public class IfCode : Code
	{
		public ValueCode Condition { get; }
		public Code Then { get; }
		public Code Else { get; }
	}

	public class GotoCode : Code
	{
		public ValueCode Target { get; }
	}

	public class AccessCode : ValueCode
	{
		public string Reference { get; }

		public override YololValue Calculate() => throw new NotImplementedException();
	}

	public class ExternalAccessCode : ValueCode
	{
		public string Reference { get; }

		public override YololValue Calculate() => throw new NotImplementedException();
	}

	public struct YololValue
	{
		public static readonly YololValue Empty = new YololValue();

		private string stringValue;
		private decimal numValue;
		private bool notEmpty;

		public bool IsNumeric => this.stringValue == null;
		public bool IsString => this.stringValue != null;
		public bool IsEmpty => !this.notEmpty;

		public YololValue(decimal value)
		{
			this.numValue = value;
			this.stringValue = null;
			this.notEmpty = true;
		}

		public YololValue(string value)
		{
			this.numValue = 0;
			this.stringValue = value ?? throw new ArgumentNullException(nameof(value));
			this.notEmpty = true;
		}

		public static YololValue operator +(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue + b.numValue);
			else if (a.IsString && b.IsString) return new YololValue(a.stringValue + b.stringValue);
			else if (a.IsString && b.IsNumeric) return new YololValue(a.stringValue + b.numValue);
			else throw new Exception("Can not add string to number");
		}

		public static YololValue operator -(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue - b.numValue);
			else throw new Exception("Incorrect types");
		}

		public static YololValue operator *(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue * b.numValue);
			else throw new Exception("Incorrect types");
		}

		public static YololValue operator /(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue / b.numValue);
			else throw new Exception("Incorrect types");
		}

		public static YololValue operator %(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue % b.numValue);
			else throw new Exception("Incorrect types");
		}

		// TODO
		public static YololValue operator ^(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue * b.numValue);
			else throw new Exception("Incorrect types");
		}

		public static YololValue operator <(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue < b.numValue ? 1 : 0);
			else throw new Exception("Incorrect types");
		}

		public static YololValue operator >(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue > b.numValue ? 1 : 0);
			else throw new Exception("Incorrect types");
		}

		public static YololValue operator <=(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue <= b.numValue ? 1 : 0);
			else throw new Exception("Incorrect types");
		}

		public static YololValue operator >=(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			if (a.IsNumeric && b.IsNumeric) return new YololValue(a.numValue >= b.numValue ? 1 : 0);
			else throw new Exception("Incorrect types");
		}

		public static YololValue operator ==(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			return new YololValue(Compare(a, b) ? 1 : 0);
		}

		public static YololValue operator !=(YololValue a, YololValue b)
		{
			if (a.IsEmpty || b.IsEmpty) throw new Exception("Can not operate on empty values");

			return new YololValue(Compare(a, b) ? 0 : 1);
		}

		public override bool Equals(object obj) => (obj is YololValue val) && Compare(this, val);

		public override int GetHashCode() => this.IsNumeric ? this.numValue.GetHashCode() : this.stringValue.GetHashCode();

		public override string ToString() => this.IsNumeric ? this.numValue.ToString() : this.stringValue;

		private static bool Compare(YololValue a, YololValue b)
		{
			if (a.IsNumeric && b.IsNumeric) return a.numValue == b.numValue;
			else if (a.IsString && b.IsString) return a.stringValue == b.stringValue;
			else if (a.IsString && b.IsNumeric) return a.stringValue == b.numValue.ToString();
			else return a.numValue.ToString() == b.stringValue; // Num String
		}
	}
}
