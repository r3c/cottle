using System.Collections.Generic;

namespace Cottle.Stores.Monitor
{
    internal class CompatibilitySymbolUsage : ISymbolUsage
    {
        private readonly Contexts.Monitor.ISymbolUsage usage;

        public CompatibilitySymbolUsage(Contexts.Monitor.ISymbolUsage usage)
        {
            this.usage = usage;
        }

        public IReadOnlyDictionary<Value, IReadOnlyList<Contexts.Monitor.ISymbolUsage>> Fields => usage.Fields;

        public Value Value => usage.Value;
    }
}