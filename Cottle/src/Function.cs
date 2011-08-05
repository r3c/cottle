using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    public abstract class   Function
    {
        #region Methods

        internal abstract Value    Execute (Scope scope, IList<IExpression> expressions, TextWriter output);

        #endregion
    }
}
