using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Evaluators
{
    internal class SymbolEvaluator : IEvaluator
    {
        private readonly Value _symbol;

        public SymbolEvaluator(Value symbol)
        {
            _symbol = symbol;
        }

        public Value Evaluate(IStore store, TextWriter output)
        {
            if (store.TryGet(_symbol, out var value))
                return value;

            return VoidValue.Instance;
        }

        public override string ToString()
        {
            return _symbol.AsString;
        }
    }
}