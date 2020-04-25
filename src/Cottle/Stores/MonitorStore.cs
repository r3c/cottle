using System;
using System.Collections.Generic;
using System.Linq;
using Cottle.Contexts.Monitor.SymbolUsages;
using Cottle.Evaluables;
using Cottle.Stores.Monitor;

namespace Cottle.Stores
{
    [Obsolete("Use `Context.CreateMonitor` method to get a `IContext` instance and pass it at document rendering")]
    public sealed class MonitorStore : IStore
    {
        private readonly IStore _store;

        private readonly MutableSymbolUsage _usage;

        public MonitorStore(IStore store)
        {
            _store = store;
            _usage = new MutableSymbolUsage(Value.Undefined);
        }

        /// <summary>
        /// Symbol usage statistics on underlying Cottle store instance.
        /// </summary>
        public IReadOnlyDictionary<Value, IReadOnlyList<Monitor.ISymbolUsage>> Symbols =>
            _usage.Fields.ToDictionary(f => f.Key,
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
            _store.Enter();
        }

        public bool Leave()
        {
            return _store.Leave();
        }

        public void Set(Value symbol, Value value, StoreMode mode)
        {
            _store.Set(symbol, value, mode);
        }

        public bool TryGet(Value symbol, out Value value)
        {
            var result = _store.TryGet(symbol, out var original);

            if (!result)
                original = Value.Undefined;

            var child = _usage.Declare(symbol, original);

            value = Value.FromEvaluable(new MonitorEvaluable(original, child));

            return result;
        }
    }
}