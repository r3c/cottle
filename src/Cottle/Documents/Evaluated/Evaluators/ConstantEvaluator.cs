using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents.Evaluated.Evaluators
{
    internal class ConstantEvaluator : IEvaluator
    {
        private readonly Value _value;

        public ConstantEvaluator(Value value)
        {
            _value = value;
        }

        public Value Evaluate(Frame frame, TextWriter output)
        {
            return _value;
        }
    }
}