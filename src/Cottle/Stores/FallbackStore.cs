using System;

namespace Cottle.Stores
{
    [Obsolete("Use any `Context.Create*` method to get a `IContext` instance and pass it at document rendering")]
    public sealed class FallbackStore : AbstractStore
    {
        public IStore Constant { get; }

        public IStore Mutable { get; }

        public FallbackStore(IStore constant, IStore mutable)
        {
            Constant = constant;
            Mutable = mutable;
        }

        public override void Enter()
        {
            Mutable.Enter();
        }

        public override bool Leave()
        {
            return Mutable.Leave();
        }

        public override void Set(Value symbol, Value value, StoreMode mode)
        {
            Mutable.Set(symbol, value, mode);
        }

        public override bool TryGet(Value symbol, out Value value)
        {
            return Mutable.TryGet(symbol, out value) || Constant.TryGet(symbol, out value);
        }
    }
}