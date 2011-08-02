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

        public override bool    AsBoolean
        {
            get
            {
                return !string.IsNullOrEmpty (this.value);
            }
        }

        public override decimal AsNumber
        {
            get
            {
                decimal number;

                return decimal.TryParse (this.value, NumberStyles.Number, CultureInfo.InvariantCulture, out number) ? number : 0;
            }
        }

        public override string  AsString
        {
            get
            {
                return this.value;
            }
        }

        #endregion

        #region Constructors

        public  StringValue (string value) :
            base (value)
        {
        }

        #endregion

        #region Methods

        public override string  ToString ()
        {
            return string.Format ("\"{0}\"", this.value);
        }

        #endregion
    }
}
