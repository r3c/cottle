using System;
using System.Globalization;

namespace Cottle.Exceptions
{
    public sealed class ParseException : Exception
    {
        public int Column => LocationStart;

        public string Lexem { get; }

        public int Line => 0;

        public int LocationLength { get; }

        public int LocationStart { get; }

        public ParseException(int locationStart, int locationLength, string message) :
            base(message)
        {
            Lexem = string.Empty;
            LocationLength = locationLength;
            LocationStart = locationStart;
        }

        public ParseException(int locationStart, int locationLength, string lexem, string expected) :
            base(string.Format(CultureInfo.InvariantCulture,
                !string.IsNullOrEmpty(expected) ? "expected '{1}', found '{0}'" : "unexpected '{0}'", lexem, expected))
        {
            Lexem = lexem;
            LocationLength = locationLength;
            LocationStart = locationStart;
        }
    }
}