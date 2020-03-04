using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents.Evaluated.Evaluators
{
    internal class AccessEvaluator : IEvaluator
    {
        public AccessEvaluator(IEvaluator source, IEvaluator subscript)
        {
            _source = source;
            _subscript = subscript;
        }

        private readonly IEvaluator _source;

        private readonly IEvaluator _subscript;

        public Value Evaluate(Frame frame, TextWriter output)
        {
            var source = _source.Evaluate(frame, output);
            var subscript = _subscript.Evaluate(frame, output);

            return source.Fields[subscript];
        }
    }
}