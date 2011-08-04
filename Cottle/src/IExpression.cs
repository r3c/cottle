using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    interface   IExpression
    {
        #region Methods

        IValue  Evaluate (Scope scope, TextWriter output);

        string  ToString ();

        #endregion
    }
}
