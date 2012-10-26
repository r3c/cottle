using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class BooleanValue : ScalarValue<bool>
    {
        #region Constants

        public static readonly BooleanValue False = new BooleanValue (false);

        public static readonly BooleanValue True = new BooleanValue (true);

        #endregion

        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return this.value;
            }
        }

        public override IFunction   AsFunction
        {
            get
            {
                return null;
            }
        }

        public override decimal     AsNumber
        {
            get
            {
                return this.value ? 1 : 0;
            }
        }

        public override string      AsString
        {
            get
            {
                return this.value ? "true" : "false";
            }
        }

        public override ValueContent    Type
        {
            get
            {
                return ValueContent.Boolean;
            }
        }

        #endregion

        #region Constructors

        public  BooleanValue (bool value) :
            base (value)
        {
        }

        #endregion

        #region Methods

        public override int CompareTo (Value other)
        {
            return other != null ? this.comparer.Compare (this.AsBoolean, other.AsBoolean) : 1;
        }

        public override string  ToString ()
        {
            return this.value ? "1" : "0";
        }

        #endregion
    }
}
