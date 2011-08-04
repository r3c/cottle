using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle.Nodes.Generics
{
    abstract class  Node : INode
    {
        #region Methods

        public abstract void    Debug (TextWriter writer);

        public abstract void    Print (Scope scope, TextWriter writer);

        #endregion
    }
}
