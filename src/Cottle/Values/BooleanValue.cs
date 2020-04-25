using System;

namespace Cottle.Values
{
    public sealed class BooleanValue : ScalarValue<bool>
    {
        [Obsolete("Use `Value.False`")]
        public static readonly BooleanValue False = new BooleanValue(false);

        [Obsolete("Use `Value.True`")]
        public static readonly BooleanValue True = new BooleanValue(true);

        public override bool AsBoolean => Value;

        public override double AsNumber => Value ? 1 : 0;

        public override string AsString => Value ? "true" : string.Empty;

        public override ValueContent Type => ValueContent.Boolean;

        [Obsolete("Use `Value.FromBoolean()`")]
        public BooleanValue(bool value) :
            base(value, source => source.AsBoolean)
        {
        }

        public override string ToString()
        {
            return Value ? "<true>" : "<false>";
        }
    }
}