using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace   Cottle
{
    public interface    IValue
    {
        #region Properties

        bool                        AsBoolean
        {
            get;
        }

        decimal                     AsNumber
        {
            get;
        }

        string                      AsString
        {
            get;
        }

        IDictionary<string, IValue> Children
        {
            get;
        }

        #endregion

        #region Methods

        string  ToString ();

        #endregion
    }
}
