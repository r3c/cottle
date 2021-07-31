namespace Cottle.Parsers.Forward
{
    internal delegate bool KeywordParser(ForwardParser parser, ParserState state, out Statement statement);
}