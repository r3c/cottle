using System.Collections.Generic;
using System.Linq;

namespace Cottle.Stores.Monitor.SymbolUsages
{
    class MutableSymbolUsage : ISymbolUsage
    {
        public IReadOnlyDictionary<Value, IReadOnlyList<ISymbolUsage>> Fields =>
            this.fields.ToDictionary(p => p.Key, p => p.Value as IReadOnlyList<ISymbolUsage>);

        public Value Value { get; }

        private readonly Dictionary<Value, List<MutableSymbolUsage>> fields;

        public MutableSymbolUsage(Value value)
        {
            this.fields = new Dictionary<Value, List<MutableSymbolUsage>>();
            this.Value = value;
        }

        public MutableSymbolUsage Declare(Value symbol, Value value)
        {
            if (!this.fields.TryGetValue(symbol, out List<MutableSymbolUsage> usages))
            {
                usages = new List<MutableSymbolUsage>();

                this.fields[symbol] = usages;
            }

            var usage = new MutableSymbolUsage(value);

            usages.Add(usage);

            return usage;
        }
    }
}
