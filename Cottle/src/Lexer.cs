using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Exceptions;

namespace   Cottle.Lexers
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

        public LexemType    Type
        {
            get
            {
                return this.type;
            }
        }

        public string       Value
        {
            get
            {
                return this.value;
            }
        }

        #endregion

        #region Attributes

        private int         column;

        private bool        eof;

        private int         index;

        private char        last;

        private int         line;

        private LexerNext   next;

        private TextReader  reader;

        private LexemType   type;

        private string      value;

        #endregion

        #region Methods

        public bool Initialize (TextReader reader)
        {
            this.column = 1;
            this.eof = false;
            this.index = 0;
            this.last = '\0';
            this.line = 1;
            this.reader = reader;

            return this.Read ();
        }

        public void Mode (LexerMode mode)
        {
            switch (mode)
            {
                case LexerMode.BLOCK:
                    this.next = this.NextBlock;

                    break;

                case LexerMode.RAW:
                    this.next = this.NextRaw;

                    break;
            }
        }

        public void Next ()
        {
            this.next ();
        }

        private void    NextBlock ()
        {
            StringBuilder   builder;
            bool            dot;
            char            end;

            while (true)
            {
                if (this.eof)
                {
                    this.type = LexemType.EOF;
                    this.value = "<EOF>";

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

                    case '}':
                        this.type = LexemType.BRACE_END;
                        this.value = this.last.ToString ();

                        this.Read ();

                        return;

                    case '[':
                        this.type = LexemType.BRACKET_BEGIN;
                        this.value = this.last.ToString ();

                        this.Read ();

                        return;

                    case ']':
                        this.type = LexemType.BRACKET_END;
                        this.value = this.last.ToString ();

                        this.Read ();

                        return;

                    case ',':
                        this.type = LexemType.COMMA;
                        this.value = this.last.ToString ();

                        this.Read ();

                        return;

                    case ':':
                        this.type = LexemType.COLON;
                        this.value = this.last.ToString ();

                        this.Read ();

                        return;

                    case '.':
                        this.type = LexemType.DOT;
                        this.value = this.last.ToString ();

                        this.Read ();

                        return;

                    case '(':
                        this.type = LexemType.PARENTHESIS_BEGIN;
                        this.value = this.last.ToString ();

                        this.Read ();

                        return;

                    case ')':
                        this.type = LexemType.PARENTHESIS_END;
                        this.value = this.last.ToString ();

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
                        builder = new StringBuilder ();

                        do 
                        {
                            builder.Append (this.last);
                        }
                        while (this.Read () && ((this.last >= '0' && this.last <= '9') ||
                                                (this.last >= 'A' && this.last <= 'Z') ||
                                                (this.last >= 'a' && this.last <= 'z') ||
                                                (this.last == '_')));

                        this.type = LexemType.NAME;
                        this.value = builder.ToString ();

                        return;

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
                        builder = new StringBuilder ();
                        dot = false;

                        do
                        {
                            if (this.last == '.')
                                dot = true;

                            builder.Append (this.last);
                        }
                        while (this.Read () && ((this.last >= '0' && this.last <= '9') || (this.last == '.' && !dot)));

                        this.type = LexemType.NUMBER;
                        this.value = builder.ToString ();

                        return;

                    case '\'':
                    case '"':
                        builder = new StringBuilder ();
                        end = this.last;

                        while (this.Read () && this.last != end)
                        {
                            if (this.last != '\\' || this.Read ())
                                builder.Append (this.last);
                        }

                        if (this.eof)
                            throw new UnknownException (this, "unfinished string");

                        this.Read ();

                        this.type = LexemType.STRING;
                        this.value = builder.ToString ();

                        return;

                    default:
                        throw new UnknownException (this, "unexpected character");
                }
            }
        }

        private void    NextRaw ()
        {
            StringBuilder   builder;
            char            end;

            if (this.eof)
            {
                this.type = LexemType.EOF;
                this.value = "<EOF>";

                return;
            }

            switch (this.last)
            {
                case '{':
                    this.type = LexemType.BRACE_BEGIN;
                    this.value = this.last.ToString ();

                    this.Read ();

                    return;

                case '}':
                    this.type = LexemType.BRACE_END;
                    this.value = this.last.ToString ();

                    this.Read ();

                    return;

                case '|':
                    this.type = LexemType.PIPE;
                    this.value = this.last.ToString ();

                    this.Read ();

                    return;

                case '\'':
                case '"':
                    builder = new StringBuilder ();
                    end = this.last;

                    while (this.Read () && this.last != end)
                    {
                        if (this.last != '\\' || this.Read ())
                            builder.Append (this.last);
                    }

                    if (this.eof)
                        throw new UnknownException (this, "unfinished string");

                    this.Read ();

                    this.type = LexemType.STRING;
                    this.value = builder.ToString ();

                    return;

                default:
                    builder = new StringBuilder ();

                    while (!this.eof && this.last != '{' && this.last != '|' && this.last != '}')
                    {
                        if (this.last != '\\' || this.Read ())
                            builder.Append (this.last);

                        this.Read ();
                    }

                    this.type = LexemType.NAME;
                    this.value = builder.ToString ();

                    return;
            }
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

        public enum LexemType
        {
            EOF,
            BRACE_BEGIN,
            BRACE_END,
            BRACKET_BEGIN,
            BRACKET_END,
            COMMA,
            COLON,
            DOT,
            NAME,
            NUMBER,
            PIPE,
            PARENTHESIS_BEGIN,
            PARENTHESIS_END,
            STRING
        }

        public enum LexerMode
        {
            BLOCK,
            RAW
        }

        private delegate void   LexerNext ();

        #endregion
    }
}
