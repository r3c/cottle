namespace Cottle.Values
{
    public sealed class BooleanValue : ScalarValue<bool>
    {
        public static readonly BooleanValue False = new BooleanValue(false);

        public static readonly BooleanValue True = new BooleanValue(true);

        public override bool AsBoolean => Value;

        public override decimal AsNumber => Value ? 1 : 0;

        public override string AsString => Value ? "true" : string.Empty;

        public override ValueContent Type => ValueContent.Boolean;

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