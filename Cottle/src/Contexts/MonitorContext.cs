using Cottle.Contexts.Monitor;
using Cottle.Contexts.Monitor.SymbolUsages;
using Cottle.Values;

namespace Cottle.Contexts
{
    internal sealed class MonitorContext : IContext
    {
        private readonly IContext _context;

        private readonly MutableSymbolUsage _usage;

        public MonitorContext(IContext context)
        {
            _context = context;
            _usage = new MutableSymbolUsage(VoidValue.Instance);
        }

        /// <summary>
        /// Symbol usage statistics on underlying Cottle store instance.
        /// </summary>
        public ISymbolUsage Usage => _usage;

        public Value this[Value symbol]
        {
            get
            {
                var original = _context[symbol];
                var child = _usage.Declare(symbol, original);

                return new MonitorValue(original, child);
            }
        }
    }
}