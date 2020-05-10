using System;
using Cottle.Maps;

namespace Cottle.Values
{
    public sealed class FunctionValue : BaseValue
    {
        public override bool AsBoolean => false;

        public override IFunction AsFunction { get; }

        public override double AsNumber => 0;

        public override string AsString => string.Empty;

        public override IMap Fields => EmptyMap.Instance;

        public override ValueContent Type => ValueContent.Function;

        [Obsolete("Use `Value.FromFunction()`")]
        public FunctionValue(IFunction function)
        {
            AsFunction = function ?? throw new ArgumentNullException(nameof(function));
        }

        public override int CompareTo(Value other)
        {
            if (Type != other.Type)
                return ((int)Type).CompareTo((int)other.Type);

            return AsFunction.CompareTo(other.AsFunction);
        }

        public override int GetHashCode()
        {
            return AsFunction.GetHashCode();
        }

        public override string ToString()
        {
            return "<" + AsFunction + "()>";
        }
    }
}