using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Evaluators
{
    internal class SymbolEvaluator : IEvaluator
    {
        #region Attributes

        private readonly Value _symbol;

        #endregion

        #region Constructors

        public SymbolEvaluator(Value symbol)
        {
            _symbol = symbol;
        }

        #endregion

        #region Methods

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

        #endregion
    }
}