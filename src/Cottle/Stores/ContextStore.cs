namespace Cottle.Stores
{
    internal class ContextStore : IStore
    {
        private readonly IContext _context;

        private readonly IStore _store;

        public ContextStore(IContext context)
        {
            _context = context;
#pragma warning disable 618
            _store = new SimpleStore();
#pragma warning restore 618
        }

        public Value this[Value symbol]
        {
            get => _store.TryGet(symbol, out var value) ? value : _context[symbol];
            set => _store[symbol] = value;
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
            if (_store.TryGet(symbol, out value))
                return true;

            value = _context[symbol];

            return value != Value.Undefined;
        }
    }
}