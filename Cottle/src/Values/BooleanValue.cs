namespace Cottle.Values
{
    public sealed class BooleanValue : ScalarValue<bool>
    {
        #region Constructors

        public BooleanValue(bool value) :
            base(value, source => source.AsBoolean)
        {
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Value ? "<true>" : "<false>";
        }

        #endregion

        #region Constants

        public static readonly BooleanValue False = new BooleanValue(false);

        public static readonly BooleanValue True = new BooleanValue(true);

        #endregion

        #region Properties

        public override bool AsBoolean => Value;

        public override decimal AsNumber => Value ? 1 : 0;

        public override string AsString => Value ? "true" : string.Empty;

        public override ValueContent Type => ValueContent.Boolean;

        #endregion
    }
}