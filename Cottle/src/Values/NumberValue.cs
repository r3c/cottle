using System.Globalization;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class NumberValue : ScalarValue<decimal>
    {
        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return this.value != 0;
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
                return this.value;
            }
        }

        public override string      AsString
        {
            get
            {
                return this.value.ToString (CultureInfo.InvariantCulture);
            }
        }

        public override ValueContent    Type
        {
            get
            {
                return ValueContent.Number;
            }
        }

        #endregion

        #region Constructors

        public  NumberValue (byte value) :
            base (value)
        {
        }

        public  NumberValue (decimal value) :
            base (value)
        {
        }

        public  NumberValue (double value) :
            base ((decimal)value)
        {
        }

        public  NumberValue (float value) :
            base ((decimal)value)
        {
        }

        public  NumberValue (int value) :
            base (value)
        {
        }

        public  NumberValue (long value) :
            base (value)
        {
        }

        public  NumberValue (short value) :
            base (value)
        {
        }

        #endregion

        #region Methods

        public override int CompareTo (Value other)
        {
            return other != null ? this.comparer.Compare (this.AsNumber, other.AsNumber) : 1;
        }

        public override string  ToString ()
        {
            return this.value.ToString (CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
