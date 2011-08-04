using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    class   RawNode : Node
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

        public override void    Debug (TextWriter writer)
        {
            writer.Write (this.text);
        }

        public override void    Print (Scope scope, TextWriter writer)
        {
            writer.Write (this.text);
        }

        #endregion
    }
}
