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

        public bool Render (Scope scope, TextWriter output, out Value result)
        {
            output.Write (this.expression.Evaluate (scope, output).AsString);

            result = UndefinedValue.Instance;

            return false;
        }

        public void Source (ISetting setting, TextWriter output)
        {
            output.Write (setting.BlockBegin);
            output.Write ("echo ");
            output.Write (this.expression);
            output.Write (setting.BlockEnd);
        }

        #endregion
    }
}
