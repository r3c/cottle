using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    sealed class    RawNode : Node
    {
        #region Attributes

        private string  text;

        #endregion

        #region Constructors

        public  RawNode (string text)
        {
            this.text = text;
        }

        #endregion

        #region Methods

        public override IValue  Apply (Scope scope, TextWriter output)
        {
            output.Write (this.text);

            return null;
        }

        public override void    Debug (TextWriter output)
        {
            output.Write (this.text);
        }

        #endregion
    }
}
