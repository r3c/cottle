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

        public void Print (ISetting setting, TextWriter output)
        {
            StringBuilder   builder;

            builder = new StringBuilder (this.text);
            builder.Replace ("\\", "\\\\");
            builder.Replace (setting.BlockBegin, "\\" + setting.BlockBegin);
            builder.Replace (setting.BlockContinue, "\\" + setting.BlockContinue);
            builder.Replace (setting.BlockEnd, "\\" + setting.BlockEnd);

            output.Write (builder.ToString ());
        }

        #endregion
    }
}
