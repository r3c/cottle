using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Values;

namespace Cottle.Functions
{
    public sealed class NativeFunction : IFunction
    {
        #region Constructors / Private

        private NativeFunction(Func<IReadOnlyList<Value>, IStore, TextWriter, Value> callback, int min, int max,
            bool pure)
        {
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            _max = max;
            _min = min;
            Pure = pure;
        }

        #endregion

        #region Properties

        public bool Pure { get; }

        #endregion

        #region Attributes

        private readonly Func<IReadOnlyList<Value>, IStore, TextWriter, Value> _callback;

        private readonly int _max;

        private readonly int _min;

        #endregion

        #region Constructors / Public

        public NativeFunction(Func<IReadOnlyList<Value>, IStore, TextWriter, Value> callback, int min, int max) :
            this(callback, min, max, false)
        {
        }

        public NativeFunction(Func<IReadOnlyList<Value>, IStore, TextWriter, Value> callback, int exact) :
            this(callback, exact, exact, false)
        {
        }

        public NativeFunction(Func<IReadOnlyList<Value>, IStore, TextWriter, Value> callback) :
            this(callback, 0, -1, false)
        {
        }

        public NativeFunction(Func<IReadOnlyList<Value>, IStore, Value> callback, int min, int max) :
            this((v, s, o) => callback(v, s), min, max, false)
        {
        }

        public NativeFunction(Func<IReadOnlyList<Value>, IStore, Value> callback, int exact) :
            this((v, s, o) => callback(v, s), exact, exact, false)
        {
        }

        public NativeFunction(Func<IReadOnlyList<Value>, IStore, Value> callback) :
            this((v, s, o) => callback(v, s), 0, -1, false)
        {
        }

        public NativeFunction(Func<IReadOnlyList<Value>, Value> callback, int min, int max) :
            this((v, s, o) => callback(v), min, max, true)
        {
        }

        public NativeFunction(Func<IReadOnlyList<Value>, Value> callback, int exact) :
            this((v, s, o) => callback(v), exact, exact, true)
        {
        }

        public NativeFunction(Func<IReadOnlyList<Value>, Value> callback) :
            this((v, s, o) => callback(v), 0, -1, true)
        {
        }

        #endregion

        #region Methods

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

        public Value Execute(IReadOnlyList<Value> arguments, IStore store, TextWriter output)
        {
            if (_min > arguments.Count || _max >= 0 && _max < arguments.Count)
                return VoidValue.Instance;

            return _callback(arguments, store, output);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    (_callback.GetHashCode() & (int)0xFFFFFF00) |
                    (_max.GetHashCode() & 0x000000F0) |
                    (_min.GetHashCode() & 0x0000000F);
            }
        }

        public override string ToString()
        {
            return "native";
        }

        #endregion
    }
}