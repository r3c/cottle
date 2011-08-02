using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class BooleanValue : ScalarValue<bool>
    {
        #region Properties

        public override bool    AsBoolean
        {
            get
            {
                return this.value;
            }
        }

        public override decimal AsNumber
        {
            get
            {
                return this.value ? 1 : 0;
            }
        }

        public override string  AsString
        {
            get
            {
                return this.value ? "1" : string.Empty;
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

        public override string  ToString ()
        {
            return this.value ? "1" : "0";
        }

        #endregion
    }
}
