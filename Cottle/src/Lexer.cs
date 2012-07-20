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

        private int         column;

        private Lexem       current;

        private bool        eof;

        private int         index;

        private char        last;

        private int         line;

        private LexerNext   next;

        private TextReader  reader;

        #endregion

        #region Methods

        public bool Initialize (TextReader reader)
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
            this.current.Flush ();

            this.next ();
        }

        private void    NextBlock ()
        {
            bool    dot;
            char    end;

            while (true)
            {
                if (this.eof)
                {
                    this.current.Push ("<EOF>");
                    this.current.Type = LexemType.EOF;

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
                        this.current.Push (this.last);
                        this.current.Type = LexemType.BRACE_END;

                        this.Read ();

                        return;

                    case '[':
                        this.current.Push (this.last);
                        this.current.Type = LexemType.BRACKET_BEGIN;

                        this.Read ();

                        return;

                    case ']':
                        this.current.Push (this.last);
                        this.current.Type = LexemType.BRACKET_END;

                        this.Read ();

                        return;

                    case ',':
                        this.current.Push (this.last);
                        this.current.Type = LexemType.COMMA;

                        this.Read ();

                        return;

                    case ':':
                        this.current.Push (this.last);
                        this.current.Type = LexemType.COLON;

                        this.Read ();

                        return;

                    case '.':
                        this.current.Push (this.last);
                        this.current.Type = LexemType.DOT;

                        this.Read ();

                        return;

                    case '(':
                        this.current.Push (this.last);
                        this.current.Type = LexemType.PARENTHESIS_BEGIN;

                        this.Read ();

                        return;

                    case ')':
                        this.current.Push (this.last);
                        this.current.Type = LexemType.PARENTHESIS_END;

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
                        do 
                        {
                            this.current.Push (this.last);
                        }
                        while (this.Read () && ((this.last >= '0' && this.last <= '9') ||
                                                (this.last >= 'A' && this.last <= 'Z') ||
                                                (this.last >= 'a' && this.last <= 'z') ||
                                                (this.last == '_')));

                        this.current.Type = LexemType.LITERAL;

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
                        dot = false;

                        do
                        {
                            if (this.last == '.')
                                dot = true;

                            this.current.Push (this.last);
                        }
                        while (this.Read () && ((this.last >= '0' && this.last <= '9') || (this.last == '.' && !dot)));

                        this.current.Type = LexemType.NUMBER;

                        return;

                    case '\'':
                    case '"':
                        end = this.last;

                        while (this.Read () && this.last != end)
                        {
                            if (this.last != '\\' || this.Read ())
                                this.current.Push (this.last);
                        }

                        if (this.eof)
                            throw new UnknownException (this, "unfinished string");

                        this.Read ();

                        this.current.Type = LexemType.STRING;

                        return;

                    default:
                        throw new UnknownException (this, "unexpected character");
                }
            }
        }

        private void    NextRawPrototype ()
        {
            List<KeyValuePair<string, LexemType>>           choices;
            int                                             index;
            ICollection<KeyValuePair<string, LexemType>>    tokens;

            if (this.eof)
            {
                this.current.Type = LexemType.EOF;
                this.current.Push ("<EOF>");

                return;
            }
/*
            tokens = this.tokens;
            index = 0;

            do
            {
                choices = new List<KeyValuePair<string, LexemType>>();

                foreach (KeyValuePair<string, LexemType> choice in tokens)
                {
                    if (index < choice.Key.Length && choice.Key[index] == this.last)
                    {
                        if (index + 1 == choice.Key.Length)
                        {
                            this.current.Type = choice.Value;

                            return;
                        }
                    }
                }

                this.current.Push (this.last);

                tokens = choices;
            }
            while (this.Read());
*/
        }

        private void    NextRaw ()
        {
//          char    end;

            if (this.eof)
            {
                this.current.Type = LexemType.EOF;
                this.current.Push ("<EOF>");

                return;
            }

            switch (this.last)
            {
                case '{':
                    this.current.Push (this.last);
                    this.current.Type = LexemType.BRACE_BEGIN;

                    this.Read ();

                    return;

                case '}':
                    this.current.Push (this.last);
                    this.current.Type = LexemType.BRACE_END;

                    this.Read ();

                    return;

                case '|':
                    this.current.Push (this.last);
                    this.current.Type = LexemType.PIPE;

                    this.Read ();

                    return;

                default:
/*
                    if (this.last <= ' ')
                    {
                        end = this.last;

                        for (this.Read (); !this.eof && this.last <= ' '; this.Read ())
                            end = this.last;

                        this.current.Push (end);
                    }
*/
                    while (!this.eof && this.last != '{' && this.last != '|' && this.last != '}')
                    {
                        if (this.last != '\\' || this.Read ())
                            this.current.Push (this.last);

                        this.Read ();
                    }

//                  this.current.Clean ();
                    this.current.Type = LexemType.TEXT;

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

        private delegate void   LexerNext ();

        #endregion
    }
}
