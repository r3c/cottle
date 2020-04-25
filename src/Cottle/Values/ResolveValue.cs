namespace Cottle.Values
{
    public abstract class ResolveValue : BaseValue
    {
        public override bool AsBoolean => _lazy.AsBoolean;

        public override IFunction AsFunction => _lazy.AsFunction;

        public override double AsNumber => _lazy.AsNumber;

        public override string AsString => _lazy.AsString;

        public override IMap Fields => _lazy.Fields;

        public override ValueContent Type => _lazy.Type;

        private readonly Value _lazy;

        protected abstract Value Resolve();

        protected ResolveValue()
        {
            _lazy = Value.FromLazy(Resolve);
        }

        public override int CompareTo(Value other)
        {
            return _lazy.CompareTo(other);
        }

        public override int GetHashCode()
        {
            return _lazy.GetHashCode();
        }

        public override string ToString()
        {
            return _lazy.ToString();
        }
    }
}