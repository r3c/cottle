using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cottle.Exceptions;

namespace Cottle.Parsers.Default
{
	class Lexer
	{
		#region Properties

		public int Column
		{
			get
			{
				return this.column;
			}
		}

		public Lexem Current
		{
			get
			{
				return this.current;
			}
		}

		public int Line
		{
			get
			{
				return this.line;
			}
		}

		#endregion

		#region Attributes

		// A buffer containing more than 85000 bytes will be allocated on LOH
		private const int MaxBufferSize = 84000 / sizeof (char);

		private int column;

		private Lexem current;

		private readonly List<LexemCursor> cursors;

		private bool eof;

		private readonly char escape;

		private char last;

		private int line;

		private readonly Queue<char> pending;

		private TextReader reader;

		private readonly LexemState root;

		#endregion

		#region Constructors

		public Lexer (string blockBegin, string blockContinue, string blockEnd, char escape)
		{
			this.cursors = new List<LexemCursor> ();
			this.escape = escape;
			this.pending = new Queue<char> ();
			this.root = new LexemState ();

			if (!this.root.Store (LexemType.BlockBegin, blockBegin))
				throw new ConfigException ("blockBegin", blockBegin, "block delimiter used twice");

			if (!this.root.Store (LexemType.BlockContinue, blockContinue))
				throw new ConfigException ("blockContinue", blockContinue, "block delimiter used twice");

			if (!this.root.Store (LexemType.BlockEnd, blockEnd))
				throw new ConfigException ("blockEnd", blockEnd, "block delimiter used twice");
		}

		#endregion

		#region Methods / Public

		public void Next (LexerMode mode)
		{
			switch (mode)
			{
				case LexerMode.Block:
					this.current = this.NextBlock ();

					break;

				case LexerMode.Raw:
					this.current = this.NextRaw ();

					break;

				default:
					throw new ParseException (this.column, this.line, "<?>", "block or raw text");
			}
		}

		public bool Reset (TextReader reader)
		{
			this.column = 1;
			this.current = new Lexem ();
			this.eof = false;
			this.last = '\0';
			this.line = 1;
			this.pending.Clear ();
			this.reader = reader;

			return this.Read ();
		}

		#endregion

		#region Methods / Private

		private Lexem NextBlock ()
		{
			StringBuilder buffer;
			bool dot;
			char end;

			while (true)
			{
				if (this.eof)
					return new Lexem (LexemType.EndOfFile, "<EOF>");

				switch (this.last)
				{
					case '\n':
					case '\r':
					case '\t':
					case ' ':
						while (this.last <= ' ' && this.Read ())
							;

						break;

					case '!':
						if (this.Read () && this.last == '=')
							return this.NextChar (LexemType.NotEqual);

						return new Lexem (LexemType.Bang, string.Empty);

					case '%':
						return this.NextChar (LexemType.Percent);

					case '&':
						if (this.Read () && this.last == '&')
							return this.NextChar (LexemType.DoubleAmpersand);

						this.pending.Enqueue (this.last);
						this.last = '&';

						return new Lexem (LexemType.None, this.last.ToString ());

					case '(':
						return this.NextChar (LexemType.ParenthesisBegin);

					case ')':
						return this.NextChar (LexemType.ParenthesisEnd);

					case '*':
						return this.NextChar (LexemType.Star);

					case '+':
						return this.NextChar (LexemType.Plus);

					case ',':
						return this.NextChar (LexemType.Comma);

					case '-':
						return this.NextChar (LexemType.Minus);

					case '.':
						return this.NextChar (LexemType.Dot);

					case '/':
						return this.NextChar (LexemType.Slash);

					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						buffer = new StringBuilder ();
						dot = false;

						do
						{
							dot |= this.last == '.';

							buffer.Append (this.last);
						}
						while (this.Read () && ((this.last >= '0' && this.last <= '9') ||
						                        (this.last == '.' && !dot)));

						return new Lexem (LexemType.Number, buffer.ToString ());

					case ':':
						return this.NextChar (LexemType.Colon);

					case '<':
						if (this.Read () && this.last == '=')
							return this.NextChar (LexemType.LowerEqual);

						return new Lexem (LexemType.LowerThan, string.Empty);

					case '=':
						return this.NextChar (LexemType.Equal);

					case '>':
						if (this.Read () && this.last == '=')
							return this.NextChar (LexemType.GreaterEqual);

						return new Lexem (LexemType.GreaterThan, string.Empty);

					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
					case 'G':
					case 'H':
					case 'I':
					case 'J':
					case 'K':
					case 'L':
					case 'M':
					case 'N':
					case 'O':
					case 'P':
					case 'Q':
					case 'R':
					case 'S':
					case 'T':
					case 'U':
					case 'V':
					case 'W':
					case 'X':
					case 'Y':
					case 'Z':
					case '_':
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
					case 'g':
					case 'h':
					case 'i':
					case 'j':
					case 'k':
					case 'l':
					case 'm':
					case 'n':
					case 'o':
					case 'p':
					case 'q':
					case 'r':
					case 's':
					case 't':
					case 'u':
					case 'v':
					case 'w':
					case 'x':
					case 'y':
					case 'z':
						buffer = new StringBuilder ();

						do 
						{
							buffer.Append (this.last);
						}
						while (this.Read () && ((this.last >= '0' && this.last <= '9') ||
						                        (this.last >= 'A' && this.last <= 'Z') ||
						                        (this.last >= 'a' && this.last <= 'z') ||
						                        (this.last == '_')));

						return new Lexem (LexemType.Symbol, buffer.ToString ());

					case '|':
						if (this.Read () && this.last == '|')
							return this.NextChar (LexemType.DoublePipe);

						this.pending.Enqueue (this.last);
						this.last = '|';

						return new Lexem (LexemType.None, this.last.ToString ());

					case '[':
						return this.NextChar (LexemType.BracketBegin);

					case ']':
						return this.NextChar (LexemType.BracketEnd);

					case '\'':
					case '"':
						buffer = new StringBuilder ();
						end = this.last;

						while (this.Read () && this.last != end)
						{
							if (this.last != this.escape || this.Read ())
								buffer.Append (this.last);
						}

						if (this.eof)
							throw new ParseException (this.column, this.line, "<eof>", "end of string");

						this.Read ();

						return new Lexem (LexemType.String, buffer.ToString ());

					default:
						return new Lexem (LexemType.None, this.last.ToString ());

				}
			}
		}

		private Lexem NextChar (LexemType type)
		{
			Lexem lexem;

			lexem = new Lexem (type, this.last.ToString ());

			this.Read ();

			return lexem;
		}

		private Lexem NextRaw ()
		{
			StringBuilder buffer;
			int copy;
			int first;
			Lexem lexem;
			LexemCursor next;
			StringBuilder token;

			buffer = new StringBuilder ();

			for (; !this.eof; this.Read ())
			{
				// Escape sequence found, cancel all pending cursors
				if (this.last == this.escape && this.Read ())
				{
					foreach (LexemCursor cursor in this.cursors)
						buffer.Append (cursor.Character);

					this.cursors.Clear ();

					buffer.Append (this.last);
				}

				// Not an escape sequence, move all cursors
				else
				{
					this.cursors.Add (new LexemCursor (this.last, this.root));

					for (int candidate = 0; candidate < this.cursors.Count; ++candidate)
					{
						next = this.cursors[candidate].Move (this.last);

						this.cursors[candidate] = next;

						// No lexem matched for this cursor, continue loop
						if (next.State == null || next.State.Type == LexemType.None)
							continue;

						// Lexem matched, flush characters located before it
						for (int i = 0; i < candidate; ++i)
							buffer.Append (this.cursors[i].Character);

						// Concatenate matched characters to build matched lexem string
						token = new StringBuilder ();

						while (candidate < this.cursors.Count)
							token.Append (this.cursors[candidate++].Character);

						// Return lexem if no text was located before or enqueue otherwise
						if (buffer.Length < 1)
							lexem = new Lexem (next.State.Type, token.ToString ());
						else
						{
							for (int i = 0; i < token.Length; ++i)
								this.pending.Enqueue (token[i]);

							lexem = new Lexem (LexemType.Text, buffer.ToString ());
						}

						this.Read ();

						this.cursors.Clear ();

						return lexem;
					}

					// Remove dead cursors and shift next ones
					first = 0;

					while (first < this.cursors.Count && this.cursors[first].State == null)
						buffer.Append (this.cursors[first++].Character);

					if (first > 0)
					{
						copy = 0;

						while (first < this.cursors.Count)
							this.cursors[copy++] = this.cursors[first++];

						this.cursors.RemoveRange (copy, this.cursors.Count - copy);
					}

					// Stop appending to buffer if we're about to reach LOH size and no cursor is pending
					if (buffer.Length > MaxBufferSize && this.cursors.Count < 1)
					{
						this.Read ();

						return new Lexem (LexemType.Text, buffer.ToString ());
					}
				}
			}

			foreach (LexemCursor cursor in this.cursors)
				buffer.Append (cursor.Character);

			this.cursors.Clear ();

			if (buffer.Length > 0)
				return new Lexem (LexemType.Text, buffer.ToString ());

			return new Lexem (LexemType.EndOfFile, "<EOF>");
		}

		private bool Read ()
		{
			int value;

			if (this.eof)
				return false;

			if (this.pending.Count > 0)
			{
				this.last = this.pending.Dequeue ();

				return true;
			}

			value = this.reader.Read ();

			if (value < 0)
			{
				this.eof = true;

				return false;
			}

			this.last = (char)value;

			if (this.last == '\n')
			{
				this.column = 1;
				++this.line;
			}
			else
				++this.column;

			return true;
		}

		#endregion
	}
}
