using System.Collections.Generic;
using System.Globalization;
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

        private int                 column;

        private Lexem               current;

        private Queue<LexemCursor>  cursors;

        private bool                eof;

        private int                 index;

        private char                last;

        private int                 line;

        private Lexem               pending;

        private TextReader          reader;

        private LexemState          root;

        #endregion

        #region Constructors

        public  Lexer (LexerConfig config)
        {
            this.cursors = new Queue<LexemCursor> ();
            this.pending = new Lexem (LexemType.None, string.Empty);
            this.root = new LexemState ();
            this.root.Store (LexemType.BraceBegin, config.BlockBegin);
            this.root.Store (LexemType.Pipe, config.BlockContinue);
            this.root.Store (LexemType.BraceEnd, config.BlockEnd);
        }

        #endregion

        #region Methods / Public

        public void Next (LexerMode mode)
        {
            switch (mode)
            {
                case LexerMode.BLOCK:
                    this.current = this.NextBlock ();

                    break;

                case LexerMode.RAW:
                    this.current = this.NextRaw ();

                    break;

                default:
                    throw new UnknownException (this, "invalid mode");
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

        private Lexem   NextBlock ()
        {
            StringBuilder   buffer;
            bool            dot;
            char            end;

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

                    case '[':
                        return this.NextChar (LexemType.BracketBegin);

                    case ']':
                        return this.NextChar (LexemType.BracketEnd);

                    case ',':
                        return this.NextChar (LexemType.Comma);

                    case ':':
                        return this.NextChar (LexemType.Colon);

                    case '.':
                        return this.NextChar (LexemType.Dot);

                    case '(':
                        return this.NextChar (LexemType.ParenthesisBegin);

                    case ')':
                        return this.NextChar (LexemType.ParenthesisEnd);

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
                        buffer = new StringBuilder ();

                        do 
                        {
                            buffer.Append (this.last);
                        }
                        while (this.Read () && ((this.last >= '0' && this.last <= '9') ||
                                                (this.last >= 'A' && this.last <= 'Z') ||
                                                (this.last >= 'a' && this.last <= 'z') ||
                                                (this.last == '_')));

                        return new Lexem (LexemType.Literal, buffer.ToString ());

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
                        buffer = new StringBuilder ();
                        dot = false;

                        do
                        {
                            if (this.last == '.')
                                dot = true;

                            buffer.Append (this.last);
                        }
                        while (this.Read () && ((this.last >= '0' && this.last <= '9') ||
                                                (this.last == '.' && !dot)));

                        return new Lexem (LexemType.Number, buffer.ToString ());

                    case '\'':
                    case '"':
                        buffer = new StringBuilder ();
                        end = this.last;

                        while (this.Read () && this.last != end)
                        {
                            if (this.last != '\\' || this.Read ())
                                buffer.Append (this.last);
                        }

                        if (this.eof)
                            throw new UnknownException (this, "unfinished string");

                        this.Read ();

                        return new Lexem (LexemType.String, buffer.ToString ());

                    default:
                        return new Lexem (LexemType.None, string.Empty);
                }
            }
        }

        private Lexem   NextChar (LexemType type)
        {
            Lexem   lexem;

            lexem = new Lexem (type, this.last.ToString (CultureInfo.InvariantCulture));

            this.Read ();

            return lexem;
        }

        private Lexem   NextRaw ()
        {
            Lexem           lexem;
            StringBuilder   text;
            StringBuilder   token;
            int             trail;
            LexemType       type;

			if (this.pending.Type != LexemType.None)
			{
                lexem = this.pending;

				this.pending = new Lexem (LexemType.None, string.Empty);

				return lexem;
			}

            if (this.eof)
                return new Lexem (LexemType.EndOfFile, "<EOF>");

            text = new StringBuilder ();

            for (; !this.eof; this.Read ())
            {
                this.cursors.Enqueue (new LexemCursor (this.last, this.root));

                trail = 0;

                foreach (LexemCursor cursor in this.cursors)
                {
                    if (cursor.Move (this.last, out type))
                    {
                        while (trail-- > 0)
                            text.Append (this.cursors.Dequeue ().Character);

                        token = new StringBuilder ();

                        while (this.cursors.Count > 0)
                            token.Append (this.cursors.Dequeue ().Character);

                        lexem = new Lexem (type, token.ToString ());

                        if (text.Length > 0)
                        {
                            this.pending = lexem;

                            lexem = new Lexem (LexemType.Text, text.ToString ());
                        }

                        this.Read ();

                        return lexem;
                    }

                    ++trail;
                }

                while (this.cursors.Count > 0 && this.cursors.Peek ().State == null)
                    text.Append (this.cursors.Dequeue ().Character);
            }

            while (this.cursors.Count > 0)
                text.Append (this.cursors.Dequeue ().Character);

            return new Lexem (LexemType.Text, text.ToString ());
        }

        private bool    Read ()
        {
            int value;

            value = this.reader.Read ();

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

        private delegate Lexem  LexerNextDelegate ();

        #endregion
    }
}
