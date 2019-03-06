using System;
using Cottle.Maps;

namespace Cottle.Values
{
    public abstract class ScalarValue<T> : Value where
        T : IComparable<T>
    {
        #region Constructors

        protected ScalarValue(T value, Converter<Value, T> converter)
        {
            Converter = converter;
            Value = value;
        }

        #endregion

        #region Properties

        public override IFunction AsFunction => null;

        public override IMap Fields => EmptyMap.Instance;

        #endregion

        #region Attributes

        protected readonly Converter<Value, T> Converter;

        protected readonly T Value;

        #endregion

        #region Methods

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

        #endregion
    }
}