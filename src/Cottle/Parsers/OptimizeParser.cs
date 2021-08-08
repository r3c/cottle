using System.IO;
using System.Linq;
using Cottle.Parsers.Optimize;
using Cottle.Parsers.Optimize.Optimizers;

namespace Cottle.Parsers
{
    internal class OptimizeParser : IParser
    {
        private static readonly IOptimizer Optimizer = new RecursiveOptimizer(new DelegateOptimizer(
            statement => Modifier.StatementModifiers.Aggregate(statement, (current, transform) => transform(current)),
            expression => Modifier.ExpressionModifiers.Aggregate(expression, (current, transform) => transform(current))
        ));

        private readonly IParser _parser;

        public OptimizeParser(IParser parser)
        {
            _parser = parser;
        }

        public bool Parse(TextReader reader, ParserState state, out Statement statement)
        {
            if (!_parser.Parse(reader, state, out var original))
            {
                statement = Statement.NoOp;

                return false;
            }

            statement = OptimizeParser.Optimizer.Optimize(original);

            return true;
        }
    }
}