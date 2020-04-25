namespace Cottle.Values
{
    public abstract class BaseValue : IEvaluable
    {
        public abstract bool AsBoolean { get; }

        public abstract IFunction AsFunction { get; }

        public abstract double AsNumber { get; }

        public abstract string AsString { get; }

        public abstract IMap Fields { get; }

        public abstract ValueContent Type { get; }

        public abstract int CompareTo(Value other);

        public virtual bool Equals(Value other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is Value other && CompareTo(other) == 0;
        }

        public abstract override int GetHashCode();
    }
}