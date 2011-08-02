using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    interface   INode
    {
        #region Methods

        void    Debug (DebugWriter writer);

        void    Print (Scope scope, TextWriter writer);

        #endregion
    }
}
