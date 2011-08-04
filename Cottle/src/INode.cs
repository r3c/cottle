using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    interface   INode
    {
        #region Methods

        IValue  Apply (Scope scope, TextWriter output);

        void    Debug (TextWriter output);

        #endregion
    }
}
