using System;
using System.Collections.Generic;
using System.IO;

namespace Cottle.Functions
{
    public sealed class NativeFunction : IFunction
    {
        public bool IsPure { get; }

        private NativeFunction(Func<IReadOnlyList<Value>, IStore, TextWriter, Value> callback, int min, int max,
            bool pure)
        {
            IsPure = pure;

            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
            _max = max;
            _min = min;
        }

        private readonly Func<IReadOnlyList<Value>, IStore, TextWriter, Value> _callback;

        private readonly int _max;

        private readonly int _min;

        [Obsolete(
            "Instances of NativeFunction are incompatible with some document types (e.g. from `Document.CreateDefault`), please use one of `Function.CreateImpureFirstOrder*` or `Function.CreateImpureHigherOrder*` static methods instead")]
        public NativeFunction(Func<IReadOnlyList<Value>, IStore, TextWriter, Value> callback, int min, int max) :
            this(callback, min, max, false)
        {
        }

        [Obsolete(
            "Instances of NativeFunction are incompatible with some document types (e.g. from `Document.CreateDefault`), please use one of `Function.CreateImpureFirstOrder*` or `Function.CreateImpureHigherOrder*` static methods instead")]
        public NativeFunction(Func<IReadOnlyList<Value>, IStore, TextWriter, Value> callback, int exact) :
            this(callback, exact, exact, false)
        {
        }

        [Obsolete(
            "Instances of NativeFunction are incompatible with some document types (e.g. from `Document.CreateDefault`), please use one of `Function.CreateImpureFirstOrder*` or `Function.CreateImpureHigherOrder*` static methods instead")]
        public NativeFunction(Func<IReadOnlyList<Value>, IStore, TextWriter, Value> callback) :
            this(callback, 0, -1, false)
        {
        }

        [Obsolete(
            "Instances of NativeFunction are incompatible with some document types (e.g. from `Document.CreateDefault`), please use one of `Function.CreatePureFirstOrder*` or `Function.CreatePureHigherOrder*` static methods instead")]
        public NativeFunction(Func<IReadOnlyList<Value>, IStore, Value> callback, int min, int max) :
            this((v, s, _) => callback(v, s), min, max, false)
        {
        }

        [Obsolete(
            "Instances of NativeFunction are incompatible with some document types (e.g. from `Document.CreateDefault`), please use one of `Function.CreatePureFirstOrder*` or `Function.CreatePureHigherOrder*` static methods instead")]
        public NativeFunction(Func<IReadOnlyList<Value>, IStore, Value> callback, int exact) :
            this((v, s, _) => callback(v, s), exact, exact, false)
        {
        }

        [Obsolete(
            "Instances of NativeFunction are incompatible with some document types (e.g. from `Document.CreateDefault`), please use one of `Function.CreatePureFirstOrder*` or `Function.CreatePureHigherOrder*` static methods instead")]
        public NativeFunction(Func<IReadOnlyList<Value>, IStore, Value> callback) :
            this((v, s, _) => callback(v, s), 0, -1, false)
        {
        }

        [Obsolete(
            "Instances of NativeFunction are incompatible with some document types (e.g. from `Document.CreateDefault`), please use one of `Function.CreatePureFirstOrder*` or `Function.CreatePureHigherOrder*` static methods instead")]
        public NativeFunction(Func<IReadOnlyList<Value>, Value> callback, int min, int max) :
            this((v, _, _) => callback(v), min, max, true)
        {
        }

        [Obsolete(
            "Instances of NativeFunction are incompatible with some document types (e.g. from `Document.CreateDefault`), please use one of `Function.CreatePureFirstOrder*` or `Function.CreatePureHigherOrder*` static methods instead")]
        public NativeFunction(Func<IReadOnlyList<Value>, Value> callback, int exact) :
            this((v, _, _) => callback(v), exact, exact, true)
        {
        }

        [Obsolete(
            "Instances of NativeFunction are incompatible with some document types (e.g. from `Document.CreateDefault`), please use one of `Function.CreatePureFirstOrder*` or `Function.CreatePureHigherOrder*` static methods instead")]
        public NativeFunction(Func<IReadOnlyList<Value>, Value> callback) :
            this((v, _, _) => callback(v), 0, -1, true)
        {
        }

        public int CompareTo(IFunction? other)
        {
            return object.ReferenceEquals(this, other) ? 0 : 1;
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
            if (!(state is IStore store))
                throw new InvalidOperationException(
                    "You're calling an instance of `NativeFunction` from an incompatible document type. Please replace all usages of `NativeFunction` by calls to `Function.Create*` static methods.");

            if (_min > arguments.Count || _max >= 0 && _max < arguments.Count)
                return Value.Undefined;

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
    }
}