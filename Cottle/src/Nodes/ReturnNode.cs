using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    sealed class    ReturnNode : Node
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

        public override IValue  Apply (Scope scope, TextWriter output)
        {
            return this.expression.Evaluate (scope, output);
        }

        public override void    Debug (TextWriter output)
        {
            output.Write ("{return ");
            output.Write (this.expression);
            output.Write ('}');
        }

        #endregion
    }
}
