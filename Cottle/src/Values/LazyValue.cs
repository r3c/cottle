using System;

namespace Cottle.Values
{
    public sealed class LazyValue : ResolveValue
    {
        #region Attributes

        private readonly Func<Value> _resolver;

        #endregion

        #region Constructors

        public LazyValue(Func<Value> resolver)
        {
            _resolver = resolver;
        }

        #endregion

        #region Methods

        protected override Value Resolve()
        {
            return _resolver();
        }

        #endregion
    }
}