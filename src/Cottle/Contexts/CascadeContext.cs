using Cottle.Values;

namespace Cottle.Contexts
{
    /// <summary>
    /// Read-only context with fallback support between two underlying context instances.
    /// </summary>
    internal class CascadeContext : IContext
    {
        private readonly IContext _fallback;

        private readonly IContext _primary;

        public CascadeContext(IContext primary, IContext fallback)
        {
            _fallback = fallback;
            _primary = primary;
        }

        public Value this[Value symbol]
        {
            get
            {
                var first = _primary[symbol];

                return first != VoidValue.Instance ? first : _fallback[symbol];
            }
        }
    }
}