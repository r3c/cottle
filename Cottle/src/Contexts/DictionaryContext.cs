using System.Collections.Generic;
using Cottle.Values;

namespace Cottle.Contexts
{
    internal class DictionaryContext : IContext
    {
        public Value this[Value symbol] => this.symbols.TryGetValue(symbol, out var value) ? value : VoidValue.Instance;

        private readonly IReadOnlyDictionary<Value, Value> symbols;

        public DictionaryContext(IReadOnlyDictionary<Value, Value> symbols)
        {
            this.symbols = symbols;
        }
    }
}