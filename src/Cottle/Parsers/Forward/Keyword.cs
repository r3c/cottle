namespace Cottle.Parsers.Forward
{
    internal readonly struct Keyword
    {
        public readonly bool HasMandatoryOperand;
        public readonly KeywordParser Parse;

        public Keyword(KeywordParser parse, bool hasMandatoryOperand)
        {
            HasMandatoryOperand = hasMandatoryOperand;
            Parse = parse;
        }
    }
}