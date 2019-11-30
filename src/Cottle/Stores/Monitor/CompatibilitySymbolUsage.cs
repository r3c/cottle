using System.Collections.Generic;

namespace Cottle.Stores.Monitor
{
    internal class CompatibilitySymbolUsage : ISymbolUsage
    {
        private readonly Contexts.Monitor.ISymbolUsage _usage;

        public CompatibilitySymbolUsage(Contexts.Monitor.ISymbolUsage usage)
        {
            _usage = usage;
        }

        public IReadOnlyDictionary<Value, IReadOnlyList<Contexts.Monitor.ISymbolUsage>> Fields => _usage.Fields;

        public Value Value => _usage.Value;

        public IReadOnlyDictionary<Value, Contexts.Monitor.ISymbolUsage> GroupFieldUsages()
        {
            return _usage.GroupFieldUsages();
        }
    }
}