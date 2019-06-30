namespace Cottle
{
    public interface IStore : IContext
    {
        new Value this[Value symbol] { get; set; }

        void Enter();

        bool Leave();

        void Set(Value symbol, Value value, StoreMode mode);

        bool TryGet(Value symbol, out Value value);
    }
}