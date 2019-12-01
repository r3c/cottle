namespace Cottle.Parsers.Forward
{
    internal readonly struct Lexem
    {
        public readonly int Length;

        public readonly int Offset;

        public readonly LexemType Type;

        public readonly string Value;

        public Lexem(LexemType type, int offset, int length, string value)
        {
            Length = length;
            Offset = offset;
            Type = type;
            Value = value;
        }
    }
}