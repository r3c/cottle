using System;
using System.Collections.Generic;
using System.Linq;
using Cottle.Contexts.Monitor;
using Cottle.Contexts.Monitor.SymbolUsages;
using Cottle.Stores.Monitor;
using Cottle.Values;

namespace Cottle.Stores
{
    [Obsolete("Use `Context.CreateMonitor` method to get a `IContext` instance and pass it at document rendering")]
    public sealed class MonitorStore : IStore
    {
        private readonly IStore store;

        private readonly MutableSymbolUsage usage;

        public MonitorStore(IStore store)
        {
            this.store = store;
            usage = new MutableSymbolUsage(VoidValue.Instance);
        }

        /// <summary>
        /// Symbol usage statistics on underlying Cottle store instance.
        /// </summary>
        public IReadOnlyDictionary<Value, IReadOnlyList<Monitor.ISymbolUsage>> Symbols =>
            usage.Fields.ToDictionary(f => f.Key,
                f => f.Value.Select(v => new CompatibilitySymbolUsage(v)).ToArray() as
                    IReadOnlyList<Monitor.ISymbolUsage>);

        public Value this[Value symbol]
        {
            get
            {
                // Output value is always defined by this implementation, see
                // method `TryGet` for details.
                TryGet(symbol, out var value);

                return value;
            }
            set => Set(symbol, value, StoreMode.Global);
        }

        public void Enter()
        {
            store.Enter();
        }

        public bool Leave()
        {
            return store.Leave();
        }

        public void Set(Value symbol, Value value, StoreMode mode)
        {
            store.Set(symbol, value, mode);
        }

        public bool TryGet(Value symbol, out Value value)
        {
            var result = store.TryGet(symbol, out var original);

            if (!result)
                original = VoidValue.Instance;

            var child = usage.Declare(symbol, original);

            value = new MonitorValue(original, child);

            return result;
        }
    }
}