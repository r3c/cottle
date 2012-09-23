using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    TextNode : INode
    {
        #region Attributes

        private string  text;

        #endregion

        #region Constructors

        public  TextNode (string text)
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

        public void Print (LexerConfig config, TextWriter output)
        {
        	StringBuilder	builder;

        	builder = new StringBuilder (this.text);
        	builder.Replace ("\\", "\\\\");
        	builder.Replace (config.BlockBegin, "\\" + config.BlockBegin);
        	builder.Replace (config.BlockContinue, "\\" + config.BlockContinue);
        	builder.Replace (config.BlockEnd, "\\" + config.BlockEnd);

        	output.Write (builder.ToString ());
        }

        #endregion
    }
}
