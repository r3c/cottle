using System.Collections.Generic;
using System.IO;
using System.Text;
using Cottle.Exceptions;

namespace Cottle.Parsers.Default
{
    internal class Lexer
    {
        #region Constructors

        public Lexer(string blockBegin, string blockContinue, string blockEnd, char escape)
        {
            _cursors = new List<LexemCursor>();
            _escape = escape;
            _pending = new Queue<char>();
            _root = new LexemState();

            if (!_root.Store(LexemType.BlockBegin, blockBegin))
                throw new ConfigException("blockBegin", blockBegin, "block delimiter used twice");

            if (!_root.Store(LexemType.BlockContinue, blockContinue))
                throw new ConfigException("blockContinue", blockContinue, "block delimiter used twice");

            if (!_root.Store(LexemType.BlockEnd, blockEnd))
                throw new ConfigException("blockEnd", blockEnd, "block delimiter used twice");
        }

        #endregion

        #region Properties

        public int Column { get; private set; }

        public Lexem Current { get; private set; }

        public int Line { get; private set; }

        #endregion

        #region Attributes

        // A buffer containing more than 85000 bytes will be allocated on LOH
        private const int MaxBufferSize = 84000 / sizeof(char);

        private readonly List<LexemCursor> _cursors;

        private bool _eof;

        private readonly char _escape;

        private char _last;

        private readonly Queue<char> _pending;

        private TextReader _reader;

        private readonly LexemState _root;

        #endregion

        #region Methods / Public

        public void Next(LexerMode mode)
        {
            switch (mode)
            {
                case LexerMode.Block:
                    Current = NextBlock();

                    break;

                case LexerMode.Raw:
                    Current = NextRaw();

                    break;

                default:
                    throw new ParseException(Column, Line, "<?>", "block or raw text");
            }
        }

        public bool Reset(TextReader reader)
        {
            Column = 1;
            Current = new Lexem();
            _eof = false;
            _last = '\0';
            Line = 1;
            _pending.Clear();
            _reader = reader;

            return Read();
        }

        #endregion

        #region Methods / Private

        private Lexem NextBlock()
        {
            while (true)
            {
                if (_eof)
                    return new Lexem(LexemType.EndOfFile, "<EOF>");

                StringBuilder buffer;
                switch (_last)
                {
                    case '\n':
                    case '\r':
                    case '\t':
                    case ' ':
                        while (_last <= ' ' && Read())
                        {
                        }

                        break;

                    case '!':
                        if (Read() && _last == '=')
                            return NextChar(LexemType.NotEqual);

                        return new Lexem(LexemType.Bang, string.Empty);

                    case '%':
                        return NextChar(LexemType.Percent);

                    case '&':
                        if (Read() && _last == '&')
                            return NextChar(LexemType.DoubleAmpersand);

                        _pending.Enqueue(_last);
                        _last = '&';

                        return new Lexem(LexemType.None, _last.ToString());

                    case '(':
                        return NextChar(LexemType.ParenthesisBegin);

                    case ')':
                        return NextChar(LexemType.ParenthesisEnd);

                    case '*':
                        return NextChar(LexemType.Star);

                    case '+':
                        return NextChar(LexemType.Plus);

                    case ',':
                        return NextChar(LexemType.Comma);

                    case '-':
                        return NextChar(LexemType.Minus);

                    case '.':
                        return NextChar(LexemType.Dot);

                    case '/':
                        return NextChar(LexemType.Slash);

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
                        buffer = new StringBuilder();
                        var dot = false;

                        do
                        {
                            dot |= _last == '.';

                            buffer.Append(_last);
                        } while (Read() && (_last >= '0' && _last <= '9' ||
                                            _last == '.' && !dot));

                        return new Lexem(LexemType.Number, buffer.ToString());

                    case ':':
                        return NextChar(LexemType.Colon);

                    case '<':
                        if (Read() && _last == '=')
                            return NextChar(LexemType.LowerEqual);

                        return new Lexem(LexemType.LowerThan, string.Empty);

                    case '=':
                        return NextChar(LexemType.Equal);

                    case '>':
                        if (Read() && _last == '=')
                            return NextChar(LexemType.GreaterEqual);

                        return new Lexem(LexemType.GreaterThan, string.Empty);

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
                        buffer = new StringBuilder();

                        do
                        {
                            buffer.Append(_last);
                        } while (Read() && (_last >= '0' && _last <= '9' ||
                                            _last >= 'A' && _last <= 'Z' ||
                                            _last >= 'a' && _last <= 'z' ||
                                            _last == '_'));

                        return new Lexem(LexemType.Symbol, buffer.ToString());

                    case '|':
                        if (Read() && _last == '|')
                            return NextChar(LexemType.DoublePipe);

                        _pending.Enqueue(_last);
                        _last = '|';

                        return new Lexem(LexemType.None, _last.ToString());

                    case '[':
                        return NextChar(LexemType.BracketBegin);

                    case ']':
                        return NextChar(LexemType.BracketEnd);

                    case '\'':
                    case '"':
                        buffer = new StringBuilder();
                        var end = _last;

                        while (Read() && _last != end)
                            if (_last != _escape || Read())
                                buffer.Append(_last);

                        if (_eof)
                            throw new ParseException(Column, Line, "<eof>", "end of string");

                        Read();

                        return new Lexem(LexemType.String, buffer.ToString());

                    default:
                        return new Lexem(LexemType.None, _last.ToString());
                }
            }
        }

        private Lexem NextChar(LexemType type)
        {
            var lexem = new Lexem(type, _last.ToString());

            Read();

            return lexem;
        }

        private Lexem NextRaw()
        {
            var buffer = new StringBuilder();

            for (; !_eof; Read())
                // Escape sequence found, cancel all pending cursors
                if (_last == _escape && Read())
                {
                    foreach (var cursor in _cursors)
                        buffer.Append(cursor.Character);

                    _cursors.Clear();

                    buffer.Append(_last);
                }

                // Not an escape sequence, move all cursors
                else
                {
                    _cursors.Add(new LexemCursor(_last, _root));

                    for (var candidate = 0; candidate < _cursors.Count; ++candidate)
                    {
                        var next = _cursors[candidate].Move(_last);

                        _cursors[candidate] = next;

                        // No lexem matched for this cursor, continue loop
                        if (next.State == null || next.State.Type == LexemType.None)
                            continue;

                        // Lexem matched, flush characters located before it
                        for (var i = 0; i < candidate; ++i)
                            buffer.Append(_cursors[i].Character);

                        // Concatenate matched characters to build matched lexem string
                        var token = new StringBuilder();

                        while (candidate < _cursors.Count)
                            token.Append(_cursors[candidate++].Character);

                        // Return lexem if no text was located before or enqueue otherwise
                        Lexem lexem;
                        if (buffer.Length < 1)
                        {
                            lexem = new Lexem(next.State.Type, token.ToString());
                        }
                        else
                        {
                            for (var i = 0; i < token.Length; ++i)
                                _pending.Enqueue(token[i]);

                            lexem = new Lexem(LexemType.Text, buffer.ToString());
                        }

                        Read();

                        _cursors.Clear();

                        return lexem;
                    }

                    // Remove dead cursors and shift next ones
                    var first = 0;

                    while (first < _cursors.Count && _cursors[first].State == null)
                        buffer.Append(_cursors[first++].Character);

                    if (first > 0)
                    {
                        var copy = 0;

                        while (first < _cursors.Count)
                            _cursors[copy++] = _cursors[first++];

                        _cursors.RemoveRange(copy, _cursors.Count - copy);
                    }

                    // Stop appending to buffer if we're about to reach LOH size and no cursor is pending
                    if (buffer.Length > MaxBufferSize && _cursors.Count < 1)
                    {
                        Read();

                        return new Lexem(LexemType.Text, buffer.ToString());
                    }
                }

            foreach (var cursor in _cursors)
                buffer.Append(cursor.Character);

            _cursors.Clear();

            if (buffer.Length > 0)
                return new Lexem(LexemType.Text, buffer.ToString());

            return new Lexem(LexemType.EndOfFile, "<EOF>");
        }

        private bool Read()
        {
            if (_eof)
                return false;

            if (_pending.Count > 0)
            {
                _last = _pending.Dequeue();

                return true;
            }

            var value = _reader.Read();

            if (value < 0)
            {
                _eof = true;

                return false;
            }

            _last = (char) value;

            if (_last == '\n')
            {
                Column = 1;
                ++Line;
            }
            else
            {
                ++Column;
            }

            return true;
        }

        #endregion
    }
}