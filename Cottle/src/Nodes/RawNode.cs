using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    RawNode : INode
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

        public bool Apply (Scope scope, TextWriter output, out Value result)
        {
            output.Write (this.text);

            result = UndefinedValue.Instance;

            return false;
        }

        public void Debug (TextWriter output)
        {
            output.Write (this.text);
        }

        #endregion
    }
}
