using System;
using System.Collections.Generic;
using System.Text;

namespace YololTest
{
	public struct YololValue
	{
		public static readonly YololValue Empty = new YololValue();

		public static readonly decimal MIN = -922337203685477.5808m;
		public static readonly decimal MAX = 922337203685477.5807m;

		private string stringValue;
		private decimal numValue;
		private bool notEmpty;

		public bool IsNumeric => this.stringValue == null;
		public bool IsString => this.stringValue != null;
		public bool IsEmpty => !this.notEmpty;

		public bool IsTrue => this.IsNumeric && this.numValue != 0;

		public bool IsInteger => this.IsNumeric && this.numValue % 1 == 0;

		public int Floored => this.IsNumeric ? (int)Math.Floor(this.numValue) : throw new Exception("Can not floor not numeric value");

		public int IntegerValue => this.IsNumeric ? (int)this.numValue : throw new Exception("Can not convert to integer");

		public YololValue(decimal value)
		{
			var clamped = value > MAX ? MAX : value < MIN ? MIN : value;
			var digitClamped = Math.Round(clamped, 4);
			this.numValue = digitClamped;
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

			if (a.IsNumeric && b.IsNumeric) return new YololValue((decimal)Math.Pow((double)a.numValue, (double)b.numValue));
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
