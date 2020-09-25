using System.IO;

namespace Cottle
{
    internal interface IParser
    {
        bool Parse(TextReader reader, ParserState state, out Statement statement);
    }
}