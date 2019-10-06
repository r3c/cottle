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
            return store[_symbol];
        }

        public override string ToString()
        {
            return _symbol.AsString;
        }
    }
}