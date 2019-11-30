namespace Cottle
{
    public interface IContext
    {
        Value this[Value symbol] { get; }
    }
}