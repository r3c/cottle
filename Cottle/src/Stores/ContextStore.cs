namespace Cottle.Stores
{
    internal class ContextStore : IStore
    {
        private readonly IContext context;

        private readonly IStore store;

        public ContextStore(IContext context)
        {
            this.context = context;
            store = new SimpleStore();
        }

        public Value this[Value symbol]
        {
            get => store.TryGet(symbol, out var value) && value.Type != ValueContent.Void
                ? value
                : context[symbol];
            set => store[symbol] = value;
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
            if (store.TryGet(symbol, out value) && value.Type != ValueContent.Void)
                return true;

            value = context[symbol];

            return value.Type != ValueContent.Void;
        }
    }
}