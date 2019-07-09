using System.Globalization;

namespace Cottle.Values
{
    public sealed class NumberValue : ScalarValue<decimal>
    {
        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override bool AsBoolean => Value != 0;

        public override decimal AsNumber => Value;

        public override string AsString => Value.ToString(CultureInfo.InvariantCulture);

        public override ValueContent Type => ValueContent.Number;

        public NumberValue(byte value) :
            this((decimal)value)
        {
        }

        public NumberValue(decimal value) :
            base(value, source => source.AsNumber)
        {
        }

        public NumberValue(double value) :
            this((decimal)value)
        {
        }

        public NumberValue(float value) :
            this((decimal)value)
        {
        }

        public NumberValue(int value) :
            this((decimal)value)
        {
        }

        public NumberValue(long value) :
            this((decimal)value)
        {
        }

        public NumberValue(short value) :
            this((decimal)value)
        {
        }
    }
}