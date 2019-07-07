using System.Collections.Generic;
using System.Linq;

namespace Cottle.Contexts.Monitor.SymbolUsages
{
    internal class MutableSymbolUsage : ISymbolUsage
    {
        public IReadOnlyDictionary<Value, IReadOnlyList<ISymbolUsage>> Fields =>
            _fields.ToDictionary(p => p.Key, p => p.Value as IReadOnlyList<ISymbolUsage>);

        public Value Value { get; }

        private readonly Dictionary<Value, List<MutableSymbolUsage>> _fields;

        public MutableSymbolUsage(Value value)
        {
            _fields = new Dictionary<Value, List<MutableSymbolUsage>>();
            Value = value;
        }

        public MutableSymbolUsage Declare(Value symbol, Value value)
        {
            if (!_fields.TryGetValue(symbol, out var usages))
            {
                usages = new List<MutableSymbolUsage>();

                _fields[symbol] = usages;
            }

            var usage = new MutableSymbolUsage(value);

            usages.Add(usage);

            return usage;
        }
    }
}