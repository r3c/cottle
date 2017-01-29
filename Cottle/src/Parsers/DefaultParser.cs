using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Builtins;
using Cottle.Exceptions;
using Cottle.Parsers.Default;
using Cottle.Values;

namespace Cottle.Parsers
{
	class DefaultParser : IParser
	{
		#region Attributes / Instance

		private readonly Lexer lexer;

		#endregion

		#region Attributes / Static

		private static readonly Dictionary<string, Func<DefaultParser, Command>> keywords = new Dictionary<string, Func<DefaultParser, Command>>
		{
			{"_",		(p) => p.ParseKeywordComment ()},
			{"declare",	(p) => p.ParseKeywordDeclare ()},
			{"define",	(p) => p.ParseKeywordSet ()},
			{"dump",	(p) => p.ParseKeywordDump ()},
			{"echo",	(p) => p.ParseKeywordEcho ()},
			{"for",		(p) => p.ParseKeywordFor ()},
			{"if",		(p) => p.ParseKeywordIf ()},
			{"return",	(p) => p.ParseKeywordReturn ()},
			{"set",		(p) => p.ParseKeywordSet ()},
			{"while",	(p) => p.ParseKeywordWhile ()}
		};

		#endregion

		#region Constructors

		public DefaultParser (string blockBegin, string blockContinue, string blockEnd, char escape)
		{
			this.lexer = new Lexer (blockBegin, blockContinue, blockEnd, escape);
		}

		#endregion

		#region Methods / Public

		public Command Parse (TextReader reader)
		{
			Command command;

			this.lexer.Reset (reader);
			this.lexer.Next (LexerMode.Raw);

			command = this.ParseCommand ();

			if (this.lexer.Current.Type != LexemType.EndOfFile)
				throw this.Raise ("end of file");

			return command;
		}

		#endregion

		#region Methods / Private

		private Expression BuildOperator (IFunction function, params Expression[] arguments)
		{
			return new Expression
			{
				Arguments = arguments,
				Source = new Expression
				{
					Type = ExpressionType.Constant,
					Value = new FunctionValue (function)
				},
				Type = ExpressionType.Invoke
			};
		}

		private Command ParseAssignment (StoreMode mode)
		{
			List<string> arguments;
			Func<StoreMode, Command> command;
			string name;

			arguments = new List<string> ();
			name = this.ParseSymbol ();

			switch (this.lexer.Current.Type)
			{
				case LexemType.ParenthesisBegin:
					arguments = new List<string> ();

					for (this.lexer.Next (LexerMode.Block); this.lexer.Current.Type != LexemType.ParenthesisEnd; )
					{
						arguments.Add (this.ParseSymbol ());

						if (this.lexer.Current.Type == LexemType.Comma)
							this.lexer.Next (LexerMode.Block);
					}

					this.lexer.Next (LexerMode.Block);

					command = (m) => new Command
					{
						Arguments = arguments.ToArray (),
						Body = this.ParseBody (),
						Mode = m,
						Name = name,
						Type = CommandType.AssignFunction
					};

					break;

				default:
					command = (m) => new Command
					{
						Mode = m,
						Name = name,
						Operand = this.ParseOperand (),
						Type = CommandType.AssignValue
					};

					break;
			}

			switch (this.lexer.Current.Type)
			{
				case LexemType.Symbol:
					if (mode == StoreMode.Global)
					{
						// <TODO> remove legacy keywords handling
						// FIXME: should raise event
						if (this.lexer.Current.Content == "as")
						{
							this.lexer.Next (LexerMode.Block);

							mode = StoreMode.Local;
						}
						else
						// </TODO>
							this.ParseExpected (LexemType.Symbol, "to", "'to' keyword");
					}
					else
						this.ParseExpected (LexemType.Symbol, "as", "'as' keyword");

					return command (mode);

				default:
					this.lexer.Next (LexerMode.Raw);

					return new Command
					{
						Mode = mode,
						Operand = Expression.Empty,
						Type = CommandType.AssignValue,
						Name = name
					};
			}
		}

		private Command ParseBody ()
		{
			if (this.lexer.Current.Type != LexemType.Colon)
				this.Raise ("body separator (':')");

			this.lexer.Next (LexerMode.Raw);

			return this.ParseCommand ();
		}

		private Command ParseCommand ()
		{
			Command current;
			Command head;
			Func<DefaultParser, Command> parse;
			Command tail;

			head = null;
			tail = null;

			while
			(
				this.lexer.Current.Type != LexemType.BlockContinue &&
			    this.lexer.Current.Type != LexemType.BlockEnd &&
			    this.lexer.Current.Type != LexemType.EndOfFile
		   	)
			{
				// Parse next block or exit loop
				switch (this.lexer.Current.Type)
				{
					case LexemType.BlockBegin:
						this.lexer.Next (LexerMode.Block);

						if (this.lexer.Current.Type == LexemType.Symbol && DefaultParser.keywords.TryGetValue (this.lexer.Current.Content, out parse))
							this.lexer.Next (LexerMode.Block);
						else
							parse = (p) => p.ParseKeywordEcho ();
			
						current = parse (this);

						if (this.lexer.Current.Type != LexemType.BlockEnd)
							throw this.Raise ("end of block");

						this.lexer.Next (LexerMode.Raw);

						break;

					case LexemType.Text:
						current = new Command
						{
							Text = this.lexer.Current.Content,
							Type = CommandType.Literal 
						};

						this.lexer.Next (LexerMode.Raw);

						break;

					default:
						throw this.Raise ("text or block begin ('{')");
				}

				// Ignore empty blocks
				if (current == null)
					continue;

				// Chain current block to parent
				if (tail != null)
				{
					tail.Next = new Command
					{
						Body = tail.Next,
						Next = current,
						Type = CommandType.Composite
					};
	
					tail = tail.Next;
				}
				else if (head != null)
				{
					tail = new Command
					{
						Body = head,
						Next = current,
						Type = CommandType.Composite
					};
	
					head = tail;
				}
				else
					head = current;
			}

			return head ?? new Command
			{
				Text = string.Empty,
				Type = CommandType.Literal
			};
		}

		private void ParseExpected (LexemType type, string value, string expected)
		{
			if (this.lexer.Current.Type != type || this.lexer.Current.Content != value)
				throw this.Raise (expected);

			this.lexer.Next (LexerMode.Block);
		}

		private Expression ParseExpression ()
		{
			Operator current;
			Stack<Expression> operands;
			Stack<Operator> operators;
			Operator other;
			Expression value;

			operands = new Stack<Expression> ();
			operators = new Stack<Operator> ();

			while (true)
			{
				operands.Push (this.ParseValue ());

				switch (this.lexer.Current.Type)
				{
					case LexemType.DoubleAmpersand:
						current = new Operator
						{
							Function = BuiltinOperators.operatorAnd,
							Precedence = 0
						};

						break;

					case LexemType.DoublePipe:
						current = new Operator
						{
							Function = BuiltinOperators.operatorOr,
							Precedence = 0
						};

						break;

					case LexemType.Equal:
						current = new Operator
						{
							Function = BuiltinOperators.operatorEqual,
							Precedence = 1
						};

						break;

					case LexemType.GreaterEqual:
						current = new Operator
						{
							Function = BuiltinOperators.operatorGreaterEqual,
							Precedence = 1
						};

						break;

					case LexemType.GreaterThan:
						current = new Operator
						{
							Function = BuiltinOperators.operatorGreaterThan,
							Precedence = 1
						};

						break;

					case LexemType.LowerEqual:
						current = new Operator
						{
							Function = BuiltinOperators.operatorLowerEqual,
							Precedence = 1
						};

						break;

					case LexemType.LowerThan:
						current = new Operator
						{
							Function = BuiltinOperators.operatorLowerThan,
							Precedence = 1
						};

						break;

					case LexemType.Minus:
						current = new Operator
						{
							Function = BuiltinOperators.operatorSub,
							Precedence = 2
						};

						break;

					case LexemType.NotEqual:
						current = new Operator
						{
							Function = BuiltinOperators.operatorNotEqual,
							Precedence = 1
						};

						break;

					case LexemType.Percent:
						current = new Operator
						{
							Function = BuiltinOperators.operatorMod,
							Precedence = 3
						};

						break;

					case LexemType.Plus:
						current = new Operator
						{
							Function = BuiltinOperators.operatorAdd,
							Precedence = 2
						};

						break;

					case LexemType.Slash:
						current = new Operator
						{
							Function = BuiltinOperators.operatorDiv,
							Precedence = 3
						};

						break;

					case LexemType.Star:
						current = new Operator
						{
							Function = BuiltinOperators.operatorMul,
							Precedence = 3
						};

						break;

					default:
						while (operators.Count > 0)
						{
							current = operators.Pop ();
							value = operands.Pop ();

							operands.Push (this.BuildOperator (current.Function, operands.Pop (), value));
						}

						return operands.Pop ();
				}

				this.lexer.Next (LexerMode.Block);

				while (operators.Count > 0 && operators.Peek ().Precedence >= current.Precedence)
				{
					other = operators.Pop ();
					value = operands.Pop ();

					operands.Push (this.BuildOperator (other.Function, operands.Pop (), value));
				}

				operators.Push (current);
			}
		}

		private Command ParseKeywordComment ()
		{
			do
			{
				this.lexer.Next (LexerMode.Raw);
			}
			while (this.lexer.Current.Type == LexemType.Text);

			return null;
		}

		private Command ParseKeywordDeclare ()
		{
			return this.ParseAssignment (StoreMode.Local);
		}

		private Command ParseKeywordDump ()
		{
			return new Command
			{
				Operand = this.ParseOperand (),
				Type = CommandType.Dump 
			};
		}

		private Command ParseKeywordEcho ()
		{
			return new Command
			{
				Operand = this.ParseOperand (),
				Type = CommandType.Echo 
			};
		}

		private Command ParseKeywordFor ()
		{
			Command body;
			Command empty;
			Expression from;
			string key;
			string value;

			key = this.ParseSymbol ();

			if (this.lexer.Current.Type == LexemType.Comma)
			{
				this.lexer.Next (LexerMode.Block);

				value = this.ParseSymbol ();
			}
			else
			{
				value = key;
				key = string.Empty;
			}

			this.ParseExpected (LexemType.Symbol, "in", "'in' keyword");

			from = this.ParseExpression ();
			body = this.ParseBody ();

			if (this.lexer.Current.Type == LexemType.BlockContinue)
			{
				this.lexer.Next (LexerMode.Block);

				this.ParseExpected (LexemType.Symbol, "empty", "'empty' keyword");

				empty = this.ParseBody ();
			}
			else
				empty = null;

			return new Command
			{
				Body = body,
				Key = key,
				Name = value,
				Next = empty,
				Operand = from,
				Type = CommandType.For
			};
		}

		private Command ParseKeywordIf ()
		{
			Expression condition;
			Command current;
			Command result;

			condition = this.ParseExpression ();
			result = new Command
			{
				Body = this.ParseBody (),
				Operand = condition,
				Type = CommandType.If
			};

			current = result;

			while (current.Next == null && this.lexer.Current.Type == LexemType.BlockContinue)
			{
				this.lexer.Next (LexerMode.Block);

				switch (this.lexer.Current.Type == LexemType.Symbol ? this.lexer.Current.Content : string.Empty)
				{
					case "elif":
						this.lexer.Next (LexerMode.Block);

						condition = this.ParseExpression ();

						current.Next = new Command
						{
							Body = this.ParseBody (),
							Operand = condition,
							Type = CommandType.If
						};

						current = current.Next;

						break;

					case "else":
						this.lexer.Next (LexerMode.Block);

						current.Next = this.ParseBody ();

						break;

					default:
						throw this.Raise ("'elif' or 'else' keyword");
				}
			}

			return result;
		}

		private Command ParseKeywordReturn ()
		{
			return new Command
			{
				Operand = this.ParseOperand (),
				Type = CommandType.Return 
			};
		}

		private Command ParseKeywordSet ()
		{
			return this.ParseAssignment (StoreMode.Global);
		}

		private Command ParseKeywordWhile ()
		{
			Command body;
			Expression condition;

			condition = this.ParseExpression ();
			body = this.ParseBody ();

			return new Command
			{
				Body = body,
				Operand = condition,
				Type = CommandType.While
			};
		}

		private Expression ParseOperand ()
		{
			Expression expression;

			expression = this.ParseExpression ();

			this.lexer.Next (LexerMode.Raw);

			return expression;
		}

		private string ParseSymbol ()
		{
			string name;

			if (this.lexer.Current.Type != LexemType.Symbol)
				throw this.Raise ("symbol (variable name)");

			name = this.lexer.Current.Content;

			this.lexer.Next (LexerMode.Block);

			return name;
		}

		private Expression ParseValue ()
		{
			List<Expression> arguments;
			List<ExpressionElement> elements;
			Expression expression;
			int index;
			Expression key;
			decimal number;
			Expression value;

			switch (this.lexer.Current.Type)
			{
				case LexemType.Bang:
					this.lexer.Next (LexerMode.Block);

					return this.BuildOperator (BuiltinOperators.operatorNot, this.ParseValue ());

				case LexemType.BracketBegin:
					elements = new List<ExpressionElement> ();
					index = 0;

					for (this.lexer.Next (LexerMode.Block); this.lexer.Current.Type != LexemType.BracketEnd; )
					{
						key = this.ParseExpression ();

						if (this.lexer.Current.Type == LexemType.Colon)
						{
							this.lexer.Next (LexerMode.Block);

							value = this.ParseExpression ();
						}
						else
						{
							value = key;
							key = new Expression
							{
								Type = ExpressionType.Constant,
								Value = index++,
							};
						}

						elements.Add (new ExpressionElement
						{
							Key = key,
							Value = value
						});

						if (this.lexer.Current.Type == LexemType.Comma)
							this.lexer.Next (LexerMode.Block);
					}

					expression = new Expression
					{
						Elements = elements.ToArray (),
						Type = ExpressionType.Map
					};

					this.lexer.Next (LexerMode.Block);

					break;

				case LexemType.Minus:
					this.lexer.Next (LexerMode.Block);

					expression = new Expression
					{
						Type = ExpressionType.Constant,
						Value = 0
					};

					return this.BuildOperator (BuiltinOperators.operatorSub, expression, this.ParseValue ());

				case LexemType.Number:
					if (!decimal.TryParse (this.lexer.Current.Content, NumberStyles.Number, CultureInfo.InvariantCulture, out number))
						number = 0;

					expression = new Expression
					{
						Type = ExpressionType.Constant,
						Value = number
					};

					this.lexer.Next (LexerMode.Block);

					break;

				case LexemType.ParenthesisBegin:
					this.lexer.Next (LexerMode.Block);

					expression = this.ParseExpression ();

					if (this.lexer.Current.Type != LexemType.ParenthesisEnd)
						throw this.Raise ("parenthesis end (')')");

					this.lexer.Next (LexerMode.Block);

					return expression;

				case LexemType.Plus:
					this.lexer.Next (LexerMode.Block);

					return this.ParseValue ();

				case LexemType.String:
					expression = new Expression
					{
						Type = ExpressionType.Constant,
						Value = this.lexer.Current.Content
					};

					this.lexer.Next (LexerMode.Block);

					break;

				case LexemType.Symbol:
					expression = new Expression
					{
						Type = ExpressionType.Symbol,
						Value = this.lexer.Current.Content
					};

					this.lexer.Next (LexerMode.Block);

					break;

				default:
					throw this.Raise ("expression");
			}

			while (true)
			{
				switch (this.lexer.Current.Type)
				{
					case LexemType.BracketBegin:
						this.lexer.Next (LexerMode.Block);

						value = this.ParseExpression ();

						if (this.lexer.Current.Type != LexemType.BracketEnd)
							throw this.Raise ("array index end (']')");

						this.lexer.Next (LexerMode.Block);

						expression = new Expression
						{
							Source = expression,
							Subscript = value,
							Type = ExpressionType.Access 
						};

						break;

					case LexemType.Dot:
						this.lexer.Next (LexerMode.Block);

						if (this.lexer.Current.Type != LexemType.Symbol)
							throw this.Raise ("field name");

						expression = new Expression
						{
							Source = expression,
							Subscript = new Expression
							{
								Type = ExpressionType.Constant,
								Value = this.lexer.Current.Content
							},
							Type = ExpressionType.Access 
						};

						this.lexer.Next (LexerMode.Block);

						break;

					case LexemType.ParenthesisBegin:
						arguments = new List<Expression> ();

						for (this.lexer.Next (LexerMode.Block); this.lexer.Current.Type != LexemType.ParenthesisEnd; )
						{
							arguments.Add (this.ParseExpression ());

							if (this.lexer.Current.Type == LexemType.Comma)
								this.lexer.Next (LexerMode.Block);
						}

						this.lexer.Next (LexerMode.Block);

						expression = new Expression
						{
							Arguments = arguments.ToArray (),
							Source = expression,
							Type = ExpressionType.Invoke
						};

						break;

					default:
						return expression;
				}
			}
		}

		private Exception Raise (string expected)
		{
			return new ParseException (this.lexer.Column, this.lexer.Line, this.lexer.Current.Content, expected);
		}

		#endregion
	}
}
