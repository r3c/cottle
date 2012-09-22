using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    DumpNode : INode
    {
        #region Attributes

        private IExpression expression;

        #endregion

        #region Constructors

        public  DumpNode (IExpression expression)
        {
            this.expression = expression;
        }

        #endregion

        #region Methods

        public bool Apply (Scope scope, TextWriter output, out Value result)
        {
            output.Write (this.expression.Evaluate (scope, output).ToString ());

            result = UndefinedValue.Instance;

            return false;
        }

        public void Print (LexerConfig config, TextWriter output)
        {
        	output.Write (config.BlockBegin);
            output.Write ("dump ");
            output.Write (this.expression);
            output.Write (config.BlockEnd);
        }

        #endregion
    }
}
