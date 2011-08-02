using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values.Generics
{
    public abstract class   ScalarValue<T> : EmptyValue
    {
        #region Attributes

        protected T value;

        #endregion

        #region Constructors

        protected   ScalarValue (T value)
        {
            this.value = value;
        }

        #endregion
    }
}
