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

		private static readonly Dictionary<string, Func<DefaultParser, Block>>	keywords = new Dictionary<string, Func<DefaultParser, Block>>
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

		public Block Parse (TextReader reader)
		{
			Block	statement;

			this.lexer.Reset (reader);
			this.lexer.Next (LexerMode.Raw);

			statement = this.ParseLiteral ();

			if (this.lexer.Current.Type != LexemType.EndOfFile)
				throw this.Raise ("end of file");

			return statement;
		}

		#endregion

		#region Methods / Private

		private Block ParseAssignment (ScopeMode mode)
		{
			List<string>			arguments;
			Func<ScopeMode, Block>	build;
			string					name;

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

					build = (m) => new Block
					{
						Arguments	= arguments.ToArray (),
						Body		= this.ParseBody (),
						Mode		= m,
						Type		= BlockType.AssignFunction,
						Value		= name
					};

					break;

				default:
					build = (m) => new Block
					{
						Mode	= m,
						Source	= this.ParseStatement (),
						Type	= BlockType.AssignValue,
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

					return build (mode);

				default:
					this.lexer.Next (LexerMode.Raw);

					return new Block
					{
						Mode	= mode,
						Source	= Expression.Empty,
						Type	= BlockType.AssignValue,
						Value	= name
					};
			}
		}

		private Block ParseCommand ()
		{
			Block						block;
			Func<DefaultParser, Block>	parse;

			if (this.lexer.Current.Type == LexemType.Symbol && DefaultParser.keywords.TryGetValue (this.lexer.Current.Content, out parse))
				this.lexer.Next (LexerMode.Block);
			else
				parse = (p) => p.ParseKeywordEcho ();

			block = parse (this);

			if (this.lexer.Current.Type != LexemType.BlockEnd)
				throw this.Raise ("end of block");

			this.lexer.Next (LexerMode.Raw);

			return block;
		}

		private Block ParseBody ()
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
								Number	= index++,
								Type	= ExpressionType.Number
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
						Number	= number,
						Type	= ExpressionType.Number
					};

					this.lexer.Next (LexerMode.Block);

					break;

				case LexemType.String:
					expression = new Expression
					{
						String	= this.lexer.Current.Content,
						Type	= ExpressionType.String
					};

					this.lexer.Next (LexerMode.Block);

					break;

				case LexemType.Symbol:
					expression = new Expression
					{
						String	= this.lexer.Current.Content,
						Type	= ExpressionType.Name
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
								String	= this.lexer.Current.Content,
								Type	= ExpressionType.String
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

		private Block ParseKeywordComment ()
		{
			do
			{
				this.lexer.Next (LexerMode.Raw);
			}
			while (this.lexer.Current.Type == LexemType.Text);

			return null;
		}

		private Block ParseKeywordDeclare ()
		{
			return this.ParseAssignment (ScopeMode.Local);
		}

		private Block ParseKeywordDump ()
		{
			return new Block
			{
				Source	= this.ParseStatement (),
				Type	= BlockType.Dump 
			};
		}

		private Block ParseKeywordEcho ()
		{
			return new Block
			{
				Source	= this.ParseStatement (),
				Type	= BlockType.Echo 
			};
		}

		private Block ParseKeywordFor ()
		{
			Block		body;
			Block		empty;
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

			return new Block
			{
				Body	= body,
				Key		= key,
				Next	= empty,
				Source	= from,
				Type	= BlockType.For,
				Value	= value
			};
		}

		private Block ParseKeywordIf ()
		{
			List<BlockBranch>	branches;
			Expression			condition;
			Block				fallback;

			branches = new List<BlockBranch> ();
			fallback = null;
			condition = this.ParseExpression ();

			branches.Add (new BlockBranch
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

						branches.Add (new BlockBranch
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

			return new Block
			{
				Body		= fallback,
				Branches	= branches.ToArray (),
				Type		= BlockType.Test
			};
		}

		private Block ParseKeywordReturn ()
		{
			return new Block
			{
				Source	= this.ParseStatement (),
				Type	= BlockType.Return 
			};
		}

		private Block ParseKeywordSet ()
		{
			return this.ParseAssignment (ScopeMode.Closest);
		}

		private Block ParseKeywordWhile ()
		{
			Block		body;
			Expression	condition;

			condition = this.ParseExpression ();
			body = this.ParseBody ();

			return new Block
			{
				Body	= body,
				Source	= condition,
				Type	= BlockType.While
			};
		}

		private Block ParseLiteral ()
		{
			Block	current;
			Block	first;
			Block	parent;
			int		state;
			Block	swap;

			first = new Block
			{
				Text	= string.Empty,
				Type	= BlockType.Literal
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
						current = new Block
						{
							Text	= this.lexer.Current.Content,
							Type	= BlockType.Literal 
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
						parent = new Block
						{
							Body	= first,
							Next	= current,
							Type	= BlockType.Composite
						};
	
						first = parent;
						state = 2;

						break;

					default:
						swap = new Block
						{
							Body	= parent.Next,
							Next	= current,
							Type	= BlockType.Composite
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
