using Cottle.Maps;

namespace Cottle.Values
{
    public sealed class FunctionValue : Value
    {
        #region Constructors

        public FunctionValue(IFunction function)
        {
            AsFunction = function;
        }

        #endregion

        #region Properties

        public override bool AsBoolean => false;

        public override IFunction AsFunction { get; }

        public override decimal AsNumber => 0;

        public override string AsString => string.Empty;

        public override IMap Fields => EmptyMap.Instance;

        public override ValueContent Type => ValueContent.Function;

        #endregion

        #region Methods

        public override int CompareTo(Value other)
        {
            if (other == null)
                return 1;

            if (Type != other.Type)
                return ((int) Type).CompareTo((int) other.Type);

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

        #endregion
    }
}