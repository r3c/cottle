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

        public Command Parse(TextReader reader)
        {
            return RecursiveOptimizer.Instance.Optimize(_parser.Parse(reader));
        }
    }
}