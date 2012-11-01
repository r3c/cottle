using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    interface   INode
    {
        #region Methods

        bool    Render (Scope scope, TextWriter output, out Value result);

        void    Source (ISetting setting, TextWriter output);

        #endregion
    }
}
