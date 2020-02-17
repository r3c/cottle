using System.IO;

namespace Cottle.Documents.Default.Evaluators
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