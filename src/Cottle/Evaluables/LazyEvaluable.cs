using System;
using System.Threading;

namespace Cottle.Evaluables
{
    internal class LazyEvaluable : IEvaluable
    {
        public bool AsBoolean => _lazy.Value.AsBoolean;
        public IFunction AsFunction => _lazy.Value.AsFunction;
        public double AsNumber => _lazy.Value.AsNumber;
        public string AsString => _lazy.Value.AsString;
        public IMap Fields => _lazy.Value.Fields;
        public ValueContent Type => _lazy.Value.Type;

        private readonly Lazy<Value> _lazy;

        public LazyEvaluable(Func<Value> resolver)
        {
            _lazy = new Lazy<Value>(resolver, LazyThreadSafetyMode.PublicationOnly);
        }

        public int CompareTo(Value other)
        {
            return _lazy.Value.CompareTo(other);
        }

        public override bool Equals(object obj)
        {
            return _lazy.Value.Equals(obj);
        }

        public bool Equals(Value other)
        {
            return _lazy.Value.Equals(other);
        }

        public override int GetHashCode()
        {
            return _lazy.Value.GetHashCode();
        }

        public override string ToString()
        {
            return _lazy.Value.ToString();
        }
    }
}