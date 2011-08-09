using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    public interface    IFunction
    {
        #region Methods

        Value   Execute (IList<Value> arguments, Scope scope, TextWriter output);

        #endregion
    }
}
