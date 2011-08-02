using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    class   EchoNode : Node
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

        public override void    Debug (DebugWriter writer)
        {
            writer.Write (string.Format ("{{echo {0}}}", this.expression));
        }

        public override void    Print (Scope scope, TextWriter writer)
        {
            writer.Write (this.expression.Evaluate (scope).AsString);
        }

        #endregion
    }
}
