using System.Collections.Generic;

namespace Cottle.Contexts
{
    internal class DictionaryContext : IContext
    {
        private readonly IReadOnlyDictionary<Value, Value> _symbols;

        public DictionaryContext(IReadOnlyDictionary<Value, Value> symbols)
        {
            _symbols = symbols;
        }

        public Value this[Value symbol] => _symbols.TryGetValue(symbol, out var value) ? value : Value.Undefined;
    }
}