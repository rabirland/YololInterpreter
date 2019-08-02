using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YololTest
{
	class Interpreter
	{
		const int UnaryPrecedence = 0;
		const int PowPrecedence = 1;
		const int MulDicPrecedence = 2;
		const int AddSubPrecedence = 3;
		const int LogicPrecedence = 4;
		const int SetPrecedence = 5;

		private static readonly Dictionary<string, int> precedences = new Dictionary<string, int>()
		{
			{ "++", UnaryPrecedence },
			{ "--", UnaryPrecedence },
			{ "!", UnaryPrecedence },

			{ "^", PowPrecedence },


			{ "*", MulDicPrecedence },
			{ "*=", MulDicPrecedence },
			{ "/", MulDicPrecedence },
			{ "/=", MulDicPrecedence },
			{ "%", MulDicPrecedence },

			{ "+", AddSubPrecedence },
			{ "+=", AddSubPrecedence },
			{ "-", AddSubPrecedence },
			{ "-=", AddSubPrecedence },

			{ "=", SetPrecedence },

			{ "<", LogicPrecedence },
			{ "<=", LogicPrecedence },
			{ ">", LogicPrecedence },
			{ ">=", LogicPrecedence },
			{ "!=", LogicPrecedence },
			{ "==", LogicPrecedence },
		};

		public int NextLine { get; set; }

		public void InterpretLine(Lex[] lexes, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields)
		{
			for (int i = 0; i < lexes.Length;)
			{
				i += this.InterpretFrom(lexes, 0, fields, externalFields);
			}
		}

		private int InterpretFrom(Lex[] lexes, int index, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields)
		{
			int lexesHandles;

			lexesHandles = this.InterpretIf(lexes, index, fields, externalFields);
			if (lexesHandles > 0) return lexesHandles;

			lexesHandles = this.InterpretGoto(lexes, index, fields, externalFields);
			if (lexesHandles > 0) return lexesHandles;

			lexesHandles = this.InterpretStatement(lexes, index, fields, externalFields);
			if (lexesHandles > 0) return lexesHandles;

			throw new Exception("Invalid state");

		}

		private int InterpretIf(Lex[] lexes, int index, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields)
		{
			if (lexes[index].Type == LexType.Keyword && lexes[index].Value == "if")
			{
				var lexesHandled = 1; // The "if" keyword
				lexesHandled += this.Evaluate(lexes, index + lexesHandled, fields, externalFields, out YololValue conditionValue); // The condition expression
				var supposedToThenLex = lexes[index + lexesHandled]; // This is supposed to be "then"

				if (supposedToThenLex.Type == LexType.Keyword && supposedToThenLex.Value == "then")
				{
					lexesHandled++; // The "then" keyword

					int indexOfElse = FindParenthesisClose(lexes, index + lexesHandled, "else", "if", "end");
					int indexOfEnd = FindParenthesisClose(lexes, index + lexesHandled, "end", "if", "end");

					if (indexOfEnd > 0)
					{
						if (conditionValue.IsTrue) // Then branch
						{
							this.InterpretFrom(lexes, index + lexesHandled, fields, externalFields); // Interpret the then branch
						}
						else if (indexOfElse > 0)
						{
							this.InterpretFrom(lexes, indexOfElse + 1, fields, externalFields);
						}
						return indexOfEnd - index + 1; // +1 because its a count
					}
					else
					{
						throw new Exception("Unclosed if");
					}
				}
				else
				{
					throw new Exception("Unexpected lex");
				}
			}
			else
			{
				return 0;
			}
		}

		private int InterpretGoto(Lex[] lexes, int index, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields)
		{
			if (lexes[index].Type == LexType.Keyword && lexes[index].Value == "goto")
			{
				var lexesHandles = this.Evaluate(lexes, index + 1, fields, externalFields, out YololValue nextLine);
				if (nextLine.IsNumeric)
				{
					int floored = nextLine.Floored;
					int clamped = Math.Clamp(floored, 1, 20);
					this.NextLine = clamped;
					return lexesHandles + 1; // +1 because of the keyword
				}
				else
				{
					throw new Exception("Invalid line number");
				}
			}
			else
			{
				return 0;
			}
		}

		private int InterpretStatement(Lex[] lexes, int index, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields)
		{
			if ((lexes[index].Type == LexType.Identifier || lexes[index].Type == LexType.ExternalIdentifier) && lexes[index+1].Type == LexType.DualOperator && lexes[index + 1].Value.EndsWith('='))
			{
				bool isCompound = lexes[index + 1].Value.Length == 2;
				var lexesHandled = this.Evaluate(lexes, index + 2, fields, externalFields, out YololValue val);

				var targetContainer = lexes[index].Type == LexType.ExternalIdentifier ? externalFields : fields;
				if (isCompound)
				{
					switch (lexes[index + 1].Value[0])
					{
						case '+': targetContainer[lexes[index].Value] += val; break;
						case '-': targetContainer[lexes[index].Value] -= val; break;
						case '*': targetContainer[lexes[index].Value] *= val; break;
						case '/': targetContainer[lexes[index].Value] /= val; break;
						case '%': targetContainer[lexes[index].Value] %= val; break;
						case '^': targetContainer[lexes[index].Value] ^= val; break;
						default: throw new Exception("Unknown operation");
					}
				}
				else
				{
					targetContainer[lexes[index].Value] = val;
				}

				return lexesHandled + 2; // +2 because of the target reference and operator
			}
			else
			{
				return 0;
			}
		}

		public int Evaluate(Lex[] lexes, int startIndex, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields, out YololValue value)
		{
			var steps = this.BuildPostfixStepOrder(lexes, startIndex);

			return this.Evaluate(steps, fields, externalFields, out value);
		}

		private int Evaluate(Stack<EvaluationStep> steps, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields, out YololValue value)
		{
			int lexesHandles = steps.Count;
			var evaluationStack = new Stack<EvaluationStep>();

			while (steps.Count > 0)
			{
				var step = steps.Pop();

				if (step.IsOperator)
				{
					// Simple
					if (step.Lex.Value == "+")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a + b));
					}
					else if (step.Lex.Value == "-")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a - b));
					}
					else if (step.Lex.Value == "*")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a * b));
					}
					else if (step.Lex.Value == "/")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a / b));
					}
					else if (step.Lex.Value == "^")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a ^ b));
					}
					else if (step.Lex.Value == "%")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a % b));
					}
					// Logic
					else if (step.Lex.Value == "<")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a < b));
					}
					else if (step.Lex.Value == "<=")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a <= b));
					}
					else if (step.Lex.Value == ">")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a > b));
					}
					else if (step.Lex.Value == ">=")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a >= b));
					}
					else if (step.Lex.Value == "==")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a == b));
					}
					else if (step.Lex.Value == "!=")
					{
						var operand2 = evaluationStack.Pop();
						var operand1 = evaluationStack.Pop();
						evaluationStack.Push(this.Operator(operand1, operand2, fields, externalFields, (a, b) => a != b));
					}
					else
					{
						throw new Exception("Unknown operator");
					}
				}
				// Parenthesis
				else if (step.IsOpenParenthesis)
				{
					this.Parenthesis(steps, fields, externalFields, out YololValue result);
					evaluationStack.Push(new EvaluationStep(result));
				}
				else if (step.IsOperand)
				{
					evaluationStack.Push(step);
				}
				else
				{
					throw new Exception("Unknown operation");
				}
			}

			if (evaluationStack.Count == 1 && evaluationStack.Peek().IsOperand)
			{
				value = default;
				var result = evaluationStack.Pop();
				if (result.IsValue)
				{
					value = result.Value;
				}
				else if (result.IsReference)
				{
					value = this.ExtractValue(result, fields, externalFields);
				}
				else
				{
					throw new Exception("Unexpected behaviour");
				}

				return lexesHandles; // The amount of lex handled
			}
			else
			{
				throw new Exception("Invalid expression");
			}
		}

		private Stack<EvaluationStep> BuildPostfixStepOrder(Lex[] lexes, int startIndex)
		{
			Stack<EvaluationStep> stepsStack = new Stack<EvaluationStep>();

			int parenthesisLevel = 0;

			var index = startIndex;
			while (index < lexes.Length)
			{
				Lex currentLex = lexes[index];

				var toPush = LexToEvaluateStep(currentLex);

				if (!toPush.IsEmpty && (!toPush.IsCloseParenthesis || parenthesisLevel > 0))
				{
					if (toPush.IsParenthesis)
					{
						if (toPush.IsCloseParenthesis)
						{
							List<EvaluationStep> parenthed = new List<EvaluationStep>();
							int level = 0;
							//Get everything inside the parenthesis and push it behind the preceeding operator (a+(ab+) => a(ab+)+ )
							while (level >= 0)
							{
								var step = stepsStack.Pop();
								parenthed.Insert(0, step);

								if (step.IsCloseParenthesis) level++;
								else if (step.IsOpenParenthesis) level--; // This will reduce the value to -1 when we hit the last opening bracket
							}
							parenthed.Add(toPush);
							if (stepsStack.Count > 0)
							{
								parenthed.Add(stepsStack.Pop()); // The operator that is supposed to preceed the brackets
							}

							foreach (var item in parenthed)
							{
								stepsStack.Push(item);
							}
							parenthesisLevel--;
						}
						else
						{
							stepsStack.Push(toPush);
							parenthesisLevel++;
						}
					}
					else if (toPush.IsReference || toPush.IsValue)
					{
						if (stepsStack.Count == 0 || stepsStack.Peek().IsOperator || stepsStack.Peek().IsOpenParenthesis)
						{
							List<EvaluationStep> pushList = new List<EvaluationStep>();
							pushList.Add(toPush);
							if (stepsStack.Count > 0 && !stepsStack.Peek().IsOpenParenthesis)
							{
								var op = stepsStack.Pop();
								pushList.Add(op); // Take out the operator from the stack, add it to the end (after the new operand) of the list

								while (stepsStack.Peek().IsOperator && precedences[stepsStack.Peek().Lex.Value] > precedences[op.Lex.Value]) // The the top of the stack is still an operator and has more precedence (less "priority")
								{
									var prevOp = stepsStack.Pop(); // Take it from the stack and ...
									pushList.Insert(2, prevOp); // ... put into the list at the second index, if we put the popped item to the end of the list, the operator order would reverse but we must maintain the order
								}
							}

							// Finally push the lis on the stack
							foreach (var item in pushList)
							{
								stepsStack.Push(item);
							}
						}
						else // If the previous item was a value, we can not push two value on the stack after each other
						{
							throw new Exception("Invalid state");
						}
					}
					else if (toPush.IsOperator)
					{
						stepsStack.Push(new EvaluationStep(currentLex));
					}
				}
				else // We hopped onto a non-evaluable lex (keyword for example)
				{
					break;
				}

				index++;
			}

			var ret = new Stack<EvaluationStep>(stepsStack); // Invert the stack
			return ret;
		}

		private EvaluationStep Operator(EvaluationStep step1, EvaluationStep step2, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields, Func<YololValue, YololValue, YololValue> op)
		{
			if (step1.IsOperand && step2.IsOperand)
			{
				var op1 = ExtractValue(step1, fields, externalFields);
				var op2 = ExtractValue(step2, fields, externalFields);

				var ret = new EvaluationStep(op(op1, op2));

				return ret;
			}
			else
			{
				throw new Exception("Invalid types to add");
			}
		}

		private int Parenthesis(Stack<EvaluationStep> steps, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields, out YololValue result)
		{
			Stack<EvaluationStep> tmpStack = new Stack<EvaluationStep>();
			int level = 0;
			while (!steps.Peek().IsCloseParenthesis || level > 0)
			{
				var nextStep = steps.Pop();
				tmpStack.Push(nextStep);

				if (nextStep.IsOpenParenthesis) level++;
				else if (nextStep.IsCloseParenthesis) level--;
			}

			steps.Pop(); // the closing parenthesis

			// Invert the stack
			return this.Evaluate(new Stack<EvaluationStep>(tmpStack), fields, externalFields, out result) + 1;
		}

		private YololValue ExtractValue(EvaluationStep step, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields)
		{
			return step.IsValue ? step.Value : step.IsExternalReference ? externalFields[step.Reference] : fields[step.Reference];
		}

		private void SetValue(EvaluationStep step, YololValue value, Dictionary<string, YololValue> fields, Dictionary<string, YololValue> externalFields)
		{
			if (step.IsReference)
			{
				if (step.IsExternalReference)
				{
					externalFields[step.Reference] = value;
				}
				else
				{
					fields[step.Reference] = value;
				}
			}
			else
			{
				throw new Exception("Can not set value of non-reference");
			}
		}

		private int FindParenthesisClose(Lex[] lexes, int startFrom, string toFind, string enterParenthesis, string exitParenthesis)
		{
			int level = 0;
			for (int i = startFrom; i < lexes.Length; i++)
			{
				if (toFind == lexes[i].Value && level == 0)
				{
					return i;
				}
				else if (lexes[i].Value == enterParenthesis)
				{
					level++;
				}
				else if (lexes[i].Value == exitParenthesis)
				{
					level--;
				}
			}

			return -1;
		}

		private EvaluationStep LexToEvaluateStep(Lex currentLex)
		{
			return		currentLex.Type == LexType.String
						? new EvaluationStep(new YololValue(currentLex.Value))
						: currentLex.Type == LexType.Identifier || currentLex.Type == LexType.ExternalIdentifier
							? new EvaluationStep(currentLex.Value)
								: currentLex.Type == LexType.DualOperator || currentLex.Type == LexType.UnaryOperator || currentLex.Type == LexType.OpenParenthesis || currentLex.Type == LexType.CloseParenthesis
									? new EvaluationStep(currentLex)
									: currentLex.Type == LexType.Number
										? new EvaluationStep(new YololValue(decimal.Parse(currentLex.Value)))
										: default;
		}
	}

	public struct EvaluationStep
	{
		private bool _notEmpty;

		public YololValue Value { get; }

		public Lex Lex { get; }

		public string Reference { get; }

		public bool IsValue { get; }

		public bool IsOperator { get; }

		public bool IsReference { get; }

		public bool IsOpenParenthesis { get; }
		public bool IsCloseParenthesis { get; }

		public bool IsParenthesis => this.IsOpenParenthesis || this.IsCloseParenthesis;

		public bool IsExternalReference => this.IsReference && this.Reference[0] == ':';

		public bool IsOperand => this.IsReference || this.IsValue;

		public bool IsEmpty => !this._notEmpty;

		public EvaluationStep(YololValue value)
		{
			this.Value = value;
			this.Lex = null;
			this.Reference = null;
			this.IsValue = true;
			this.IsOperator = false;
			this._notEmpty = true;
			this.IsReference = false;
			this.IsOpenParenthesis = false;
			this.IsCloseParenthesis = false;
		}

		public EvaluationStep(Lex lex)
		{
			this.Lex = lex;
			this.Value = YololValue.Empty;
			this.Reference = null;
			this.IsValue = false;
			this.IsOperator = lex.Type == LexType.DualOperator || lex.Type == LexType.UnaryOperator;
			this.IsOpenParenthesis = lex.Type == LexType.OpenParenthesis;
			this.IsCloseParenthesis = lex.Type == LexType.CloseParenthesis;
			this._notEmpty = true;
			this.IsReference = false;
		}

		public EvaluationStep(string reference)
		{
			this.Lex = null;
			this.Value = YololValue.Empty;
			this.Reference = reference;
			this.IsValue = false;
			this.IsOperator = false;
			this._notEmpty = true;
			this.IsReference = true;
			this.IsOpenParenthesis = false;
			this.IsCloseParenthesis = false;
		}

		public override string ToString() => this.IsValue ? this.Value.ToString() : this.IsReference ? this.Reference : this.Lex.ToString();
	}
}
