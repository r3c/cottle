using System;
using System.Collections.Generic;
using System.IO;

namespace Cottle.Functions
{
    using Callback0 = Func<object, TextWriter, Value>;
    using Callback1 = Func<object, Value, TextWriter, Value>;
    using Callback2 = Func<object, Value, Value, TextWriter, Value>;
    using Callback3 = Func<object, Value, Value, Value, TextWriter, Value>;

    /// <summary>
    /// Function implementation with support for fixed number of arguments (from 0 to 3). This implementation allows
    /// reducing number of allocations when invoking functions internally.
    /// </summary>
    internal class FiniteFunction : IFunction
    {
        public bool IsPure { get; }

        private static readonly Callback0 NoCallback0 = (_, _) => Value.Undefined;
        private static readonly Callback1 NoCallback1 = (_, _, _) => Value.Undefined;
        private static readonly Callback2 NoCallback2 = (_, _, _, _) => Value.Undefined;
        private static readonly Callback3 NoCallback3 = (_, _, _, _, _) => Value.Undefined;

        private readonly Callback0 _callback0;
        private readonly Callback1 _callback1;
        private readonly Callback2 _callback2;
        private readonly Callback3 _callback3;

        public FiniteFunction(bool isPure, Callback0? callback0, Callback1? callback1, Callback2? callback2,
            Callback3? callback3)
        {
            IsPure = isPure;
            _callback0 = callback0 ?? FiniteFunction.NoCallback0;
            _callback1 = callback1 ?? FiniteFunction.NoCallback1;
            _callback2 = callback2 ?? FiniteFunction.NoCallback2;
            _callback3 = callback3 ?? FiniteFunction.NoCallback3;
        }

        public int CompareTo(IFunction? other)
        {
            return ReferenceEquals(this, other) ? 0 : 1;
        }

        public bool Equals(IFunction? other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is IFunction other && Equals(other);
        }

        public Value Invoke(object state, IReadOnlyList<Value> arguments, TextWriter output)
        {
            switch (arguments.Count)
            {
                case 0:
                    return Invoke0(state, output);

                case 1:
                    return Invoke1(state, arguments[0], output);

                case 2:
                    return Invoke2(state, arguments[0], arguments[1], output);

                case 3:
                    return Invoke3(state, arguments[0], arguments[1], arguments[2], output);

                default:
                    return Value.Undefined;
            }
        }

        public Value Invoke0(object state, TextWriter output)
        {
            return _callback0.Invoke(state, output);
        }

        public Value Invoke1(object state, Value argument0, TextWriter output)
        {
            return _callback1.Invoke(state, argument0, output);
        }

        public Value Invoke2(object state, Value argument0, Value argument1, TextWriter output)
        {
            return _callback2.Invoke(state, argument0, argument1, output);
        }

        public Value Invoke3(object state, Value argument0, Value argument1, Value argument2, TextWriter output)
        {
            return _callback3.Invoke(state, argument0, argument1, argument2, output);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    ((_callback0.GetHashCode() << 24) & (int)0xFF000000) |
                    ((_callback1.GetHashCode() << 16) & 0x00FF0000) |
                    ((_callback2.GetHashCode() << 8) & 0x0000FF00) |
                    ((_callback3.GetHashCode() << 0) & 0x000000FF);
            }
        }
    }
}