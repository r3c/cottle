using System;
using System.Globalization;

namespace Cottle.Exceptions
{
    public class ParseException : Exception
    {
        #region Constructors

        public ParseException(int column, int line, string lexem, string expected) :
            base(string.Format(CultureInfo.InvariantCulture,
                !string.IsNullOrEmpty(expected) ? "expected '{1}', found '{0}'" : "unexpected '{0}'", lexem, expected))
        {
            Column = column;
            Lexem = lexem;
            Line = line;
        }

        #endregion

        #region Properties

        public int Column { get; }

        public string Lexem { get; }

        public int Line { get; }

        #endregion
    }
}