using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    interface   INode
    {
        #region Methods

        bool    Apply (Scope scope, TextWriter output, out Value result);

        void    Debug (TextWriter output);

        #endregion
    }
}
