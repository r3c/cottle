using System;
using Cottle.Maps;

namespace Cottle.Values
{
    public abstract class ScalarValue<T> : BaseValue where
        T : IComparable<T>
    {
        public override IFunction AsFunction => Function.Empty;

        public override IMap Fields => EmptyMap.Instance;

        protected readonly T Value;

        private readonly Converter<Value, T> _converter;

        protected ScalarValue(T value, Converter<Value, T> converter)
        {
            _converter = converter;
            Value = value;
        }

        public override int CompareTo(Value other)
        {
            if (Type != other.Type)
                return ((int)Type).CompareTo((int)other.Type);

            return _converter(this).CompareTo(_converter(other));
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}