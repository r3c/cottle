using System;
using System.Collections.Generic;
using System.IO;

namespace Cottle.Functions
{
    using Callback = Func<object, IReadOnlyList<Value>, TextWriter, Value>;

    /// <summary>
    /// Generic function implementation relying on injected callback for evaluation. Also provide verification of
    /// number of arguments.
    /// </summary>
    internal class CallbackFunction : IFunction
    {
        public bool IsPure { get; }

        private readonly Callback _callback;
        private readonly int _max;
        private readonly int _min;

        public CallbackFunction(bool isPure, Callback callback, int min, int max)
        {
            IsPure = isPure;
            _callback = callback;
            _max = max;
            _min = min;
        }

        public int CompareTo(IFunction other)
        {
            return object.ReferenceEquals(this, other) ? 0 : 1;
        }

        public bool Equals(IFunction other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is IFunction other && Equals(other);
        }

        public Value Invoke(object state, IReadOnlyList<Value> arguments, TextWriter output)
        {
            if (arguments.Count < _min || arguments.Count > _max)
                return Value.Undefined;

            return _callback(state, arguments, output);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    ((_callback.GetHashCode() << 8) & (int)0xFFFFFF00) |
                    ((_max.GetHashCode() << 4) & 0x000000F0) |
                    (_min.GetHashCode() & 0x0000000F);
            }
        }
    }
}