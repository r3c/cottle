using System.Threading;

namespace Cottle.Values
{
    public abstract class ResolveValue : Value
    {
        public override bool AsBoolean => Acquire().AsBoolean;

        public override IFunction AsFunction => Acquire().AsFunction;

        public override decimal AsNumber => Acquire().AsNumber;

        public override string AsString => Acquire().AsString;

        public override IMap Fields => Acquire().Fields;

        public override ValueContent Type => Acquire().Type;

        private readonly object _mutex = new object();

        private Value _value;

        protected abstract Value Resolve();

        private Value Acquire()
        {
            if (_value == null)
            {
                lock (_mutex)
                {
                    if (_value == null)
                        Interlocked.Exchange(ref _value, Resolve());
                }
            }

            return _value;
        }

        public override int CompareTo(Value other)
        {
            return Acquire().CompareTo(other);
        }

        public override int GetHashCode()
        {
            return Acquire().GetHashCode();
        }

        public override string ToString()
        {
            return Acquire().ToString();
        }
    }
}