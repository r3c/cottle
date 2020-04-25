using System;
using System.Globalization;

namespace Cottle.Values
{
    public sealed class NumberValue : ScalarValue<double>
    {
        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool AsBoolean => Math.Abs(Value) > double.Epsilon;

        public override double AsNumber => Value;

        public override string AsString => Value.ToString(CultureInfo.InvariantCulture);

        public override ValueContent Type => ValueContent.Number;

        [Obsolete("Use `Value.FromNumber()`")]
        public NumberValue(byte value) :
            this((decimal)value)
        {
        }

        [Obsolete("Use `Value.FromNumber()`")]
        public NumberValue(decimal value) :
            this((double)value)
        {
        }

        [Obsolete("Use `Value.FromNumber()`")]
        public NumberValue(double value) :
            base(value, source => source.AsNumber)
        {
        }

        [Obsolete("Use `Value.FromNumber()`")]
        public NumberValue(float value) :
            this((decimal)value)
        {
        }

        [Obsolete("Use `Value.FromNumber()`")]
        public NumberValue(int value) :
            this((decimal)value)
        {
        }

        [Obsolete("Use `Value.FromNumber()`")]
        public NumberValue(long value) :
            this((decimal)value)
        {
        }

        [Obsolete("Use `Value.FromNumber()`")]
        public NumberValue(short value) :
            this((decimal)value)
        {
        }
    }
}