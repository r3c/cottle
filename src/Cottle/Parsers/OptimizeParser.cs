using System.Collections.Generic;
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

        public bool Parse(TextReader reader, out Statement statement, out IEnumerable<DocumentReport> reports)
        {
            if (!_parser.Parse(reader, out var original, out reports))
            {
                statement = default;

                return false;
            }

            statement = RecursiveOptimizer.Instance.Optimize(original);

            return true;
        }
    }
}