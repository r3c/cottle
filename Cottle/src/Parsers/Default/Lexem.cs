namespace Cottle.Parsers.Default
{
    internal readonly struct Lexem
    {
        #region Attributes

        public readonly string Content;

        public readonly LexemType Type;

        #endregion

        #region Constructors

        public Lexem(LexemType type, string content)
        {
            Content = content;
            Type = type;
        }

        #endregion
    }
}