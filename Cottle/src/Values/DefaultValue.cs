using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class DefaultValue : EmptyValue
    {
        #region Properties

        public override bool    AsBoolean
        {
            get
            {
                return false;
            }
        }

        public override decimal AsNumber
        {
            get
            {
                return 0;
            }
        }

        public override string  AsString
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion

        #region Methods

        public override string  ToString ()
        {
            return "<default>";
        }

        #endregion
    }
}
