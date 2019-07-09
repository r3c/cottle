using Cottle.Values;

namespace Cottle.Stores
{
    public abstract class AbstractStore : IStore
    {
        public Value this[Value symbol]
        {
            get
            {
                if (TryGet(symbol, out var value))
                    return value;

                return VoidValue.Instance;
            }
            set => Set(symbol, value, StoreMode.Global);
        }

        public abstract void Enter();

        public abstract bool Leave();

        public abstract void Set(Value symbol, Value value, StoreMode mode);

        public abstract bool TryGet(Value symbol, out Value value);
    }
}