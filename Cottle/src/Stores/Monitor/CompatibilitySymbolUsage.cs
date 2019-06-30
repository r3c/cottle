using System.Collections.Generic;

namespace Cottle.Stores.Monitor
{
    internal class CompatibilitySymbolUsage : ISymbolUsage
    {
        public IReadOnlyDictionary<Value, IReadOnlyList<Contexts.Monitor.ISymbolUsage>> Fields => this.usage.Fields;

        public Value Value => this.usage.Value;

        private readonly Contexts.Monitor.ISymbolUsage usage;

        public CompatibilitySymbolUsage(Contexts.Monitor.ISymbolUsage usage)
        {
            this.usage = usage;
        }
    }
}