using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Exceptions;

namespace   Cottle
{
    class   Lexer
    {
        #region Properties

        public int          Column
        {
            get
            {
                return this.column;
            }
        }

        public Lexem        Current
        {
            get
            {
                return this.current;
            }
        }

        public int          Index
        {
            get
            {
                return this.index;
            }
        }

        public int          Line
        {
            get
            {
                return this.line;
            }
        }

        #endregion

        #region Attributes

        private LexemMatch[]    blocks;

        private int             column;

        private Lexem           current;

        private bool            eof;

        private int             index;

        private char            last;

        private int             line;

        private LexemMatch      pending;

        private TextReader      reader;

        #endregion

        #region Constructors

        public  Lexer (LexerConfig config)
        {
            this.blocks = new LexemMatch[]
            {
			    new LexemMatch (LexemType.BraceBegin, config.BlockBegin),
			    new LexemMatch (LexemType.Pipe, config.BlockContinue),
			    new LexemMatch (LexemType.BraceEnd, config.BlockEnd)
            };

            this.pending = new LexemMatch (LexemType.None, string.Empty);
        }

        #endregion

        #region Methods / Public

        public void Next (LexerMode mode)
        {
            switch (mode)
            {
                case LexerMode.BLOCK:
                    this.NextBlock ();

                    break;

                case LexerMode.RAW:
                    this.NextRaw ();

                    break;

                default:
                    throw new UnknownException (this, "invalid lexem");
            }
        }

        public bool Reset (TextReader reader)
        {
            this.column = 1;
            this.current = new Lexem ();
            this.eof = false;
            this.index = 0;
            this.last = '\0';
            this.line = 1;
            this.reader = reader;

            return this.Read ();
        }

        #endregion

        #region Methods / Private

        private void    NextBlock ()
        {
            bool    dot;
            char    end;

            while (true)
            {
                if (this.eof)
                {
                    this.current.Reset (LexemType.EndOfFile);
                    this.current.Push ("<EOF>");

                    return;
                }

                switch (this.last)
                {
                    case '\n':
                    case '\r':
                    case '\t':
                    case ' ':
                        while (this.last <= ' ' && this.Read ())
                            ;

                        break;

                    case '[':
                        this.current.Reset (LexemType.BracketBegin);
                        this.current.Push (this.last);

                        this.Read ();

                        return;

                    case ']':
                        this.current.Reset (LexemType.BracketEnd);
                        this.current.Push (this.last);

                        this.Read ();

                        return;

                    case ',':
                        this.current.Reset (LexemType.Comma);
                        this.current.Push (this.last);

                        this.Read ();

                        return;

                    case ':':
                        this.current.Reset (LexemType.Colon);
                        this.current.Push (this.last);

                        this.Read ();

                        return;

                    case '.':
                        this.current.Reset (LexemType.Dot);
                        this.current.Push (this.last);

                        this.Read ();

                        return;

                    case '(':
                        this.current.Reset (LexemType.ParenthesisBegin);
                        this.current.Push (this.last);

                        this.Read ();

                        return;

                    case ')':
                        this.current.Reset (LexemType.ParenthesisEnd);
                        this.current.Push (this.last);

                        this.Read ();

                        return;

                    case '_':
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
                        this.current.Reset (LexemType.Literal);

                        do 
                        {
                            this.current.Push (this.last);
                        }
                        while (this.Read () && ((this.last >= '0' && this.last <= '9') ||
                                                (this.last >= 'A' && this.last <= 'Z') ||
                                                (this.last >= 'a' && this.last <= 'z') ||
                                                (this.last == '_')));

                        return;

                    case '-':
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
                        this.current.Reset (LexemType.Number);

                        dot = false;

                        do
                        {
                            if (this.last == '.')
                                dot = true;

                            this.current.Push (this.last);
                        }
                        while (this.Read () && ((this.last >= '0' && this.last <= '9') || (this.last == '.' && !dot)));

                        return;

                    case '\'':
                    case '"':
                        this.current.Reset (LexemType.String);

                        end = this.last;

                        while (this.Read () && this.last != end)
                        {
                            if (this.last != '\\' || this.Read ())
                                this.current.Push (this.last);
                        }

                        if (this.eof)
                            throw new UnknownException (this, "unfinished string");

                        this.Read ();

                        return;

                    default:
                        this.current.Reset (LexemType.None);

                        return;
                }
            }
        }

        private void	NextRaw ()
        {
        	ICollection<LexemMatch> branches;
        	StringBuilder           buffer;
        	StringBuilder           builder;
        	int                     index;
        	List<LexemMatch>        trails;

			if (this.pending.Type != LexemType.None)
			{
                this.current.Reset (this.pending.Type);
				this.current.Push (this.pending.Content);

				this.pending = new LexemMatch (LexemType.None, string.Empty);

				return;
			}

            if (this.eof)
            {
                this.current.Reset (LexemType.EndOfFile);
                this.current.Push ("<EOF>");

                return;
            }

			builder = new StringBuilder ();
            buffer = new StringBuilder ();
			
			while (!this.eof)
			{
				branches = this.blocks;
				index = 0;

				buffer.Length = 0;

				do
				{
					trails = null;

					foreach (LexemMatch branch in branches)
					{
						if (index < branch.Content.Length && branch.Content[index] == this.last)
						{
							if (index + 1 == branch.Content.Length)
							{
								if (builder.Length > 0)
								{
                                    this.current.Reset (LexemType.Text);
                                    this.current.Push (builder.ToString ());

									this.pending = branch;
								}
                                else
                                {
                                    this.current.Reset (branch.Type);
								    this.current.Push (branch.Content);
                                }

								this.Read ();

								return;
							}

							if (trails == null)
								trails = new List<LexemMatch> (branches.Count);

							trails.Add (branch);
						}
					}

                    if (this.last != '\\' || this.Read ())
					    buffer.Append (this.last);

					branches = trails;

					++index;
				}
				while (this.Read () && branches != null && branches.Count > 0);

				builder.Append (buffer.ToString ());
			}

            this.current.Reset (LexemType.Text);
			this.current.Push (builder.ToString ());
        }

        private bool    Read ()
        {
            int value = this.reader.Read ();

            if (value >= 0)
            {
                this.last = (char)value;

                if (this.last == '\n')
                {
                    this.column = 1;
                    ++this.line;
                }
                else
                    ++this.column;

                ++this.index;
            }
            else
                this.eof = true;

            return !this.eof;
        }

        #endregion

        #region Types

        private delegate void   LexerNext ();

        #endregion
    }
}
