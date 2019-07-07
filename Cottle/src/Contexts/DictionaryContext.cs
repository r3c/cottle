using System.Collections.Generic;
using Cottle.Values;

namespace Cottle.Contexts
{
    internal class DictionaryContext : IContext
    {
        private readonly IReadOnlyDictionary<Value, Value> _symbols;

        public DictionaryContext(IReadOnlyDictionary<Value, Value> symbols)
        {
            _symbols = symbols;
        }

        public Value this[Value symbol] => _symbols.TryGetValue(symbol, out var value) ? value : VoidValue.Instance;
    }
}