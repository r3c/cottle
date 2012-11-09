using System.Globalization;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class NumberValue : ScalarValue<decimal>
    {
        #region Properties

        public override bool            AsBoolean
        {
            get
            {
                return this.value != 0;
            }
        }

        public override decimal         AsNumber
        {
            get
            {
                return this.value;
            }
        }

        public override string          AsString
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

        #region Constructors / Public

        public  NumberValue (byte value) :
            this ((decimal)value)
        {
        }

        public  NumberValue (decimal value) :
            base (value, delegate (Value source)
            {
                return source.AsNumber;
            })
        {
        }

        public  NumberValue (double value) :
            this ((decimal)value)
        {
        }

        public  NumberValue (float value) :
            this ((decimal)value)
        {
        }

        public  NumberValue (int value) :
            this ((decimal)value)
        {
        }

        public  NumberValue (long value) :
            this ((decimal)value)
        {
        }

        public  NumberValue (short value) :
            this ((decimal)value)
        {
        }

        #endregion

        #region Methods

        public override string  ToString ()
        {
            return this.value.ToString (CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
