using System;
using Cottle.Maps;

namespace Cottle.Values
{
    public abstract class ScalarValue<T> : Value where
        T : IComparable<T>
    {
        public override IFunction AsFunction => null;

        public override IMap Fields => EmptyMap.Instance;

        protected readonly Converter<Value, T> Converter;

        protected readonly T Value;

        protected ScalarValue(T value, Converter<Value, T> converter)
        {
            Converter = converter;
            Value = value;
        }

        public override int CompareTo(Value other)
        {
            if (other == null)
                return 1;

            if (Type != other.Type)
                return ((int)Type).CompareTo((int)other.Type);

            return Converter(this).CompareTo(Converter(other));
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}