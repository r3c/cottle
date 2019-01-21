namespace Cottle.Stores
{
    public sealed class FallbackStore : AbstractStore
    {
        #region Constructors

        public FallbackStore(IStore constant, IStore mutable)
        {
            Constant = constant;
            Mutable = mutable;
        }

        #endregion

        #region Properties

        public IStore Constant { get; }

        public IStore Mutable { get; }

        #endregion

        #region Attributes

        #endregion

        #region Methods

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

        #endregion
    }
}