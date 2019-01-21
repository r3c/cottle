using Cottle.Builtins;
using Cottle.Values;

namespace Cottle.Stores
{
    public sealed class BuiltinStore : AbstractStore
    {
        #region Attributes / Instance

        private readonly FallbackStore _store;

        #endregion

        #region Constructors

        public BuiltinStore()
        {
            _store = new FallbackStore(GetConstant(), new SimpleStore());
        }

        #endregion

        #region Methods / Private

        private static IStore GetConstant()
        {
            if (_constant == null)
                lock (Mutex)
                {
                    if (_constant == null)
                    {
                        IStore store = new SimpleStore();

                        foreach (var instance in BuiltinFunctions.Instances)
                            store[instance.Key] = new FunctionValue(instance.Value);

                        _constant = store;
                    }
                }

            return _constant;
        }

        #endregion

        #region Attributes / Static

        private static volatile IStore _constant;

        private static readonly object Mutex = new object();

        #endregion

        #region Methods / Public

        public override void Enter()
        {
            _store.Enter();
        }

        public override bool Leave()
        {
            return _store.Leave();
        }

        public override void Set(Value symbol, Value value, StoreMode mode)
        {
            _store.Set(symbol, value, mode);
        }

        public override bool TryGet(Value symbol, out Value value)
        {
            return _store.TryGet(symbol, out value);
        }

        #endregion
    }
}