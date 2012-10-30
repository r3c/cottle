using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    EchoNode : INode
    {
        #region Attributes

        private IExpression expression;

        #endregion

        #region Constructors

        public  EchoNode (IExpression expression)
        {
            this.expression = expression;
        }

        #endregion

        #region Methods

        public bool Apply (Scope scope, TextWriter output, out Value result)
        {
            output.Write (this.expression.Evaluate (scope, output).AsString);

            result = UndefinedValue.Instance;

            return false;
        }

        public void Print (LexerConfig config, TextWriter output)
        {
            output.Write (config.BlockBegin);
            output.Write ("echo ");
            output.Write (this.expression);
            output.Write (config.BlockEnd);
        }

        #endregion
    }
}
