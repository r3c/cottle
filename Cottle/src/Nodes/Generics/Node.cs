using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle.Nodes.Generics
{
    abstract class  Node : INode
    {
        #region Methods

        public abstract IValue  Apply (Scope scope, TextWriter output);

        public abstract void    Debug (TextWriter output);

        #endregion
    }
}
