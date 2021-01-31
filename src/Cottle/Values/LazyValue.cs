using System;

namespace Cottle.Values
{
    public sealed class LazyValue : ResolveValue
    {
        private readonly Func<Value> _resolver;

        [Obsolete("Use `Value.FromLazy()`")]
        public LazyValue(Func<Value> resolver)
        {
            _resolver = resolver;
        }

        protected override Value Resolve()
        {
            return _resolver();
        }
    }
}