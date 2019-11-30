using Cottle.Values;

namespace Cottle.Contexts
{
    internal class EmptyContext : IContext
    {
        public static readonly EmptyContext Instance = new EmptyContext();

        private EmptyContext()
        {
        }

        public Value this[Value symbol] => VoidValue.Instance;
    }
}