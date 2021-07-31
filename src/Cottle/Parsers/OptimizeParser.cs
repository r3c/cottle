using System.IO;
using Cottle.Parsers.Optimize.Optimizers;

namespace Cottle.Parsers
{
    internal class OptimizeParser : IParser
    {
        private readonly IParser _parser;

        public OptimizeParser(IParser parser)
        {
            _parser = parser;
        }

        public bool Parse(TextReader reader, ParserState state, out Statement statement)
        {
            if (!_parser.Parse(reader, state, out var original))
            {
                statement = default;

                return false;
            }

            statement = RecursiveOptimizer.Instance.Optimize(original);

            return true;
        }
    }
}