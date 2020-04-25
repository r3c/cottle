using System;

namespace Cottle.Contexts
{
    internal class CallbackContext : IContext
    {
        public Value this[Value symbol] => _callback(symbol);

        private readonly Func<Value, Value> _callback;

        public CallbackContext(Func<Value, Value> callback)
        {
            _callback = callback;
        }
    }
}