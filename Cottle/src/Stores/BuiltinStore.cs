using System;
using Cottle.Contexts;

namespace Cottle.Stores
{
    [Obsolete("Use `Context.CreateBuiltin` method to get a `IContext` instance and pass it at document rendering")]
    public sealed class BuiltinStore : AbstractStore
    {
        private static readonly IStore Constant = new ContextStore(BuiltinContext.Instance);

        private readonly FallbackStore store;

        public BuiltinStore()
        {
            store = new FallbackStore(BuiltinStore.Constant, new SimpleStore());
        }

        public override void Enter()
        {
            store.Enter();
        }

        public override bool Leave()
        {
            return store.Leave();
        }

        public override void Set(Value symbol, Value value, StoreMode mode)
        {
            store.Set(symbol, value, mode);
        }

        public override bool TryGet(Value symbol, out Value value)
        {
            return store.TryGet(symbol, out value);
        }
    }
}