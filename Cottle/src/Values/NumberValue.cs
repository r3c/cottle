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

        public  NumberValue (decimal value) :
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
