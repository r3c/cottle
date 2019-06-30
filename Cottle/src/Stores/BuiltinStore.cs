using Cottle.Contexts;

namespace Cottle.Stores
{
    public sealed class BuiltinStore : AbstractStore
    {
        private static readonly IStore Constant = new ContextStore(BuiltinContext.Instance);

        private readonly FallbackStore store;

        public BuiltinStore()
        {
            this.store = new FallbackStore(BuiltinStore.Constant, new SimpleStore());
        }

        public override void Enter()
        {
            this.store.Enter();
        }

        public override bool Leave()
        {
            return this.store.Leave();
        }

        public override void Set(Value symbol, Value value, StoreMode mode)
        {
            this.store.Set(symbol, value, mode);
        }

        public override bool TryGet(Value symbol, out Value value)
        {
            return this.store.TryGet(symbol, out value);
        }
    }
}