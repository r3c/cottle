namespace Cottle.Parsers.Forward
{
    internal readonly struct Lexem
    {
        public readonly string Content;

        public readonly LexemType Type;

        public Lexem(LexemType type, string content)
        {
            Content = content;
            Type = type;
        }
    }
}