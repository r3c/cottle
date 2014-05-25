using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cottle.Exceptions;
using Cottle.Parsers.Default;

namespace Cottle.Parsers
{
	class DefaultParser : IParser
	{
		#region Attributes / Instance

		private readonly Lexer	lexer;

		#endregion

		#region Attributes / Static

		private static readonly Dictionary<string, Func<DefaultParser, Command>>	keywords = new Dictionary<string, Func<DefaultParser, Command>>
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

		public DefaultParser (string blockBegin, string blockContinue, string blockEnd)
		{
			this.lexer = new Lexer (blockBegin, blockContinue, blockEnd);
		}

		#endregion

		#region Methods / Public

		public Command Parse (TextReader reader)
		{
			Command	statement;

			this.lexer.Reset (reader);
			this.lexer.Next (LexerMode.Raw);

			statement = this.ParseLiteral ();

			if (this.lexer.Current.Type != LexemType.EndOfFile)
				throw this.Raise ("end of file");

			return statement;
		}

		#endregion

		#region Methods / Private

		private Command ParseAssignment (ScopeMode mode)
		{
			List<string>				arguments;
			Func<ScopeMode, Command>	command;
			string						name;

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
						Arguments	= arguments.ToArray (),
						Body		= this.ParseBody (),
						Mode		= m,
						Type		= CommandType.AssignFunction,
						Value		= name
					};

					break;

				default:
					command = (m) => new Command
					{
						Mode	= m,
						Source	= this.ParseStatement (),
						Type	= CommandType.AssignValue,
						Value	= name
					};

					break;
			}

			switch (this.lexer.Current.Type)
			{
				case LexemType.Symbol:
					if (mode == ScopeMode.Closest)
					{
						// <TODO> remove legacy keywords handling
						// FIXME: should raise event
						if (this.lexer.Current.Content == "as")
						{
							this.lexer.Next (LexerMode.Block);

							mode = ScopeMode.Local;
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
						Mode	= mode,
						Source	= Expression.Empty,
						Type	= CommandType.AssignValue,
						Value	= name
					};
			}
		}

		private Command ParseCommand ()
		{
			Command							command;
			Func<DefaultParser, Command>	parse;

			if (this.lexer.Current.Type == LexemType.Symbol && DefaultParser.keywords.TryGetValue (this.lexer.Current.Content, out parse))
				this.lexer.Next (LexerMode.Block);
			else
				parse = (p) => p.ParseKeywordEcho ();

			command = parse (this);

			if (this.lexer.Current.Type != LexemType.BlockEnd)
				throw this.Raise ("end of block");

			this.lexer.Next (LexerMode.Raw);

			return command;
		}

		private Command ParseBody ()
		{
			if (this.lexer.Current.Type != LexemType.Colon)
				throw this.Raise ("body separator (':')");

			this.lexer.Next (LexerMode.Raw);

			return this.ParseLiteral ();
		}

		private void ParseExpected (LexemType type, string value, string expected)
		{
			if (this.lexer.Current.Type != type || this.lexer.Current.Content != value)
				throw this.Raise (expected);

			this.lexer.Next (LexerMode.Block);
		}

		private Expression ParseExpression ()
		{
			List<Expression>		arguments;
			List<ExpressionElement>	elements;
			Expression				expression;
			int						index;
			Expression				key;
			decimal					number;
			Expression				value;

			switch (this.lexer.Current.Type)
			{
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
								Type	= ExpressionType.Constant,
								Value	= index++,
							};
						}

						elements.Add (new ExpressionElement
						{
							Key		= key,
							Value	= value
						});

						if (this.lexer.Current.Type == LexemType.Comma)
							this.lexer.Next (LexerMode.Block);
					}

					expression = new Expression
					{
						Elements	= elements.ToArray (),
						Type		= ExpressionType.Map
					};

					this.lexer.Next (LexerMode.Block);

					break;

				case LexemType.Number:
					if (!decimal.TryParse (this.lexer.Current.Content, NumberStyles.Number, CultureInfo.InvariantCulture, out number))
						number = 0;

					expression = new Expression
					{
						Type	= ExpressionType.Constant,
						Value	= number
					};

					this.lexer.Next (LexerMode.Block);

					break;

				case LexemType.String:
					expression = new Expression
					{
						Type	= ExpressionType.Constant,
						Value	= this.lexer.Current.Content
					};

					this.lexer.Next (LexerMode.Block);

					break;

				case LexemType.Symbol:
					expression = new Expression
					{
						Type	= ExpressionType.Symbol,
						Value	= this.lexer.Current.Content
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
							Source		= expression,
							Subscript	= value,
							Type		= ExpressionType.Access 
						};

						break;

					case LexemType.Dot:
						this.lexer.Next (LexerMode.Block);

						if (this.lexer.Current.Type != LexemType.Symbol)
							throw this.Raise ("field name");

						expression = new Expression
						{
							Source		= expression,
							Subscript	= new Expression
							{
								Type	= ExpressionType.Constant,
								Value	= this.lexer.Current.Content
							},
							Type		= ExpressionType.Access 
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
							Arguments	= arguments.ToArray (),
							Source		= expression,
							Type		= ExpressionType.Invoke
						};

						break;

					default:
						return expression;
				}
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
			return this.ParseAssignment (ScopeMode.Local);
		}

		private Command ParseKeywordDump ()
		{
			return new Command
			{
				Source	= this.ParseStatement (),
				Type	= CommandType.Dump 
			};
		}

		private Command ParseKeywordEcho ()
		{
			return new Command
			{
				Source	= this.ParseStatement (),
				Type	= CommandType.Echo 
			};
		}

		private Command ParseKeywordFor ()
		{
			Command		body;
			Command		empty;
			Expression	from;
			string		key;
			string		value;

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
				Body	= body,
				Key		= key,
				Next	= empty,
				Source	= from,
				Type	= CommandType.For,
				Value	= value
			};
		}

		private Command ParseKeywordIf ()
		{
			List<CommandBranch>	branches;
			Expression			condition;
			Command				fallback;

			branches = new List<CommandBranch> ();
			fallback = null;
			condition = this.ParseExpression ();

			branches.Add (new CommandBranch
			{
				Body		= this.ParseBody (),
				Condition	= condition 
			});

			while (fallback == null && this.lexer.Current.Type == LexemType.BlockContinue)
			{
				this.lexer.Next (LexerMode.Block);

				switch (this.lexer.Current.Type == LexemType.Symbol ? this.lexer.Current.Content : string.Empty)
				{
					case "elif":
						this.lexer.Next (LexerMode.Block);

						condition = this.ParseExpression ();

						branches.Add (new CommandBranch
						{
							Body		= this.ParseBody (),
							Condition	= condition
						});

						break;

					case "else":
						this.lexer.Next (LexerMode.Block);

						fallback = this.ParseBody ();

						break;

					default:
						throw this.Raise ("'elif' or 'else' keyword");
				}
			}

			return new Command
			{
				Body		= fallback,
				Branches	= branches.ToArray (),
				Type		= CommandType.If
			};
		}

		private Command ParseKeywordReturn ()
		{
			return new Command
			{
				Source	= this.ParseStatement (),
				Type	= CommandType.Return 
			};
		}

		private Command ParseKeywordSet ()
		{
			return this.ParseAssignment (ScopeMode.Closest);
		}

		private Command ParseKeywordWhile ()
		{
			Command		body;
			Expression	condition;

			condition = this.ParseExpression ();
			body = this.ParseBody ();

			return new Command
			{
				Body	= body,
				Source	= condition,
				Type	= CommandType.While
			};
		}

		private Command ParseLiteral ()
		{
			Command	current;
			Command	first;
			Command	parent;
			int		state;
			Command	swap;

			first = new Command
			{
				Text	= string.Empty,
				Type	= CommandType.Literal
			};

			parent = null;
			state = 0;

			while (true)
			{
				// Parse next block or exit loop
				switch (this.lexer.Current.Type)
				{
					case LexemType.BlockBegin:
						this.lexer.Next (LexerMode.Block);

						current = this.ParseCommand ();

						break;

					case LexemType.BlockContinue:
					case LexemType.BlockEnd:
					case LexemType.EndOfFile:
						return first;

					case LexemType.Text:
						current = new Command
						{
							Text	= this.lexer.Current.Content,
							Type	= CommandType.Literal 
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
				switch (state)
				{
					case 0:
						first = current;
						state = 1;

						break;

					case 1:
						parent = new Command
						{
							Body	= first,
							Next	= current,
							Type	= CommandType.Composite
						};
	
						first = parent;
						state = 2;

						break;

					default:
						swap = new Command
						{
							Body	= parent.Next,
							Next	= current,
							Type	= CommandType.Composite
						};

						parent.Next = swap;
						parent = parent.Next;

						break;
				}
			}
		}

		private Expression ParseStatement ()
		{
			Expression	expression;

			expression = this.ParseExpression ();

			this.lexer.Next (LexerMode.Raw);

			return expression;
		}

		private string ParseSymbol ()
		{
			string	name;

			if (this.lexer.Current.Type != LexemType.Symbol)
				throw this.Raise ("variable name");

			name = this.lexer.Current.Content;

			this.lexer.Next (LexerMode.Block);

			return name;
		}

		private Exception Raise (string expected)
		{
			return new ParseException (this.lexer.Column, this.lexer.Line, this.lexer.Current.Content, expected);
		}

		#endregion
	}
}
