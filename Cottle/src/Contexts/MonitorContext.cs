using Cottle.Contexts.Monitor;
using Cottle.Contexts.Monitor.SymbolUsages;
using Cottle.Values;

namespace Cottle.Contexts
{
    internal sealed class MonitorContext : IContext
    {
        /// <summary>
        /// Symbol usage statistics on underlying Cottle store instance.
        /// </summary>
        public ISymbolUsage Usage => this.usage;

        public Value this[Value symbol]
        {
            get
            {
                var original = this.context[symbol];
                var child = this.usage.Declare(symbol, original);

                return new MonitorValue(original, child);
            }
        }

        private readonly IContext context;

        private readonly MutableSymbolUsage usage;

        public MonitorContext(IContext context)
        {
            this.context = context;
            this.usage = new MutableSymbolUsage(VoidValue.Instance);
        }
    }
}