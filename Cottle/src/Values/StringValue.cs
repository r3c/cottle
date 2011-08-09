using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class StringValue : ScalarValue<string>
    {
        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return !string.IsNullOrEmpty (this.value);
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
                decimal number;

                return decimal.TryParse (this.value, NumberStyles.Number, CultureInfo.InvariantCulture, out number) ? number : 0;
            }
        }

        public override string      AsString
        {
            get
            {
                return this.value;
            }
        }

        public override DataType    Type
        {
            get
            {
                return DataType.STRING;
            }
        }

        #endregion

        #region Constructors

        public  StringValue (string value) :
            base (value)
        {
        }

        public  StringValue (char value) :
            base (value.ToString ())
        {
        }

        #endregion

        #region Methods

        public override int CompareTo (Value other)
        {
            return this.value.CompareTo (other.AsString);
        }

        public override string  ToString ()
        {
            return string.Format ("\"{0}\"", this.value.Replace ("\\", "\\\\").Replace ("\"", "\\\""));
        }

        #endregion
    }
}
