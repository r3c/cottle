using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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

        public override Function    AsFunction
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

        #endregion

        #region Constructors

        public  NumberValue (decimal value) :
            base (value)
        {
        }

        #endregion
    }
}
