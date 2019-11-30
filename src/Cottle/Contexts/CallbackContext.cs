using System;
using Cottle.Values;

namespace Cottle.Contexts
{
    internal class CallbackContext : IContext
    {
        public Value this[Value symbol] => _callback(symbol) ?? VoidValue.Instance;

        private readonly Func<Value, Value> _callback;

        public CallbackContext(Func<Value, Value> callback)
        {
            _callback = callback;
        }
    }
}