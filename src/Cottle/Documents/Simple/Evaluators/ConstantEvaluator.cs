using System.IO;

namespace Cottle.Documents.Simple.Evaluators
{
    internal class ConstantEvaluator : IEvaluator
    {
        private readonly Value _value;

        public ConstantEvaluator(Value value)
        {
            _value = value;
        }

        public Value Evaluate(IStore store, TextWriter output)
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}