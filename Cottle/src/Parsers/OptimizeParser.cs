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

        public bool Parse(TextReader reader, out Command command, out IReadOnlyList<DocumentReport> reports)
        {
            if (!_parser.Parse(reader, out var original, out reports))
            {
                command = default;

                return false;
            }

            command = RecursiveOptimizer.Instance.Optimize(original);

            return true;
        }
    }
}