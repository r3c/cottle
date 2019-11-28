using System.IO;

namespace Cottle.Documents.Default.Evaluators
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

        public Value Evaluate(Stack stack, TextWriter output)
        {
            var source = _source.Evaluate(stack, output);
            var subscript = _subscript.Evaluate(stack, output);

            return source.Fields[subscript];
        }
    }
}