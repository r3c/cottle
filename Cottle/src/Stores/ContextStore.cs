namespace Cottle.Stores
{
    internal class ContextStore : IStore
    {
        public Value this[Value symbol]
        {
            get => this.store.TryGet(symbol, out var value) && value.Type != ValueContent.Void
                ? value
                : this.context[symbol];
            set => this.store[symbol] = value;
        }

        private readonly IContext context;

        private readonly IStore store;

        public ContextStore(IContext context)
        {
            this.context = context;
            this.store = new SimpleStore();
        }

        public void Enter()
        {
            this.store.Enter();
        }

        public bool Leave()
        {
            return this.store.Leave();
        }

        public void Set(Value symbol, Value value, StoreMode mode)
        {
            this.store.Set(symbol, value, mode);
        }

        public bool TryGet(Value symbol, out Value value)
        {
            if (this.store.TryGet(symbol, out value) && value.Type != ValueContent.Void)
                return true;

            value = this.context[symbol];

            return value.Type != ValueContent.Void;
        }
    }
}