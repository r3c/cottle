using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle.Nodes
{
    sealed class    ReturnNode : INode
    {
        #region Attributes

        private IExpression expression;

        #endregion

        #region Constructors

        public  ReturnNode (IExpression expression)
        {
            this.expression = expression;
        }

        #endregion

        #region Methods

        public bool Apply (Scope scope, TextWriter output, out Value result)
        {
            result = this.expression.Evaluate (scope, output);

            return true;
        }

        public void Print (LexerConfig config, TextWriter output)
        {
        	output.Write (config.BlockBegin);
            output.Write ("return ");
            output.Write (this.expression);
            output.Write (config.BlockEnd);
        }

        #endregion
    }
}
