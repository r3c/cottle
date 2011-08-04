using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    sealed class    WhileNode : Node
    {
        #region Attributes

        private INode       body;

        private IExpression test;

        #endregion

        #region Constructors

        public  WhileNode (IExpression test, INode body)
        {
            this.body = body;
            this.test = test;
        }

        #endregion

        #region Methods

        public override IValue  Apply (Scope scope, TextWriter output)
        {
            IValue  result;

            while (this.test.Evaluate (scope, output).AsBoolean)
            {
                result = this.body.Apply (scope, output);

                if (result != null)
                    return result;
            }

            return null;
        }

        public override void    Debug (TextWriter output)
        {
            output.Write ("{while ");
            output.Write (this.test);
            output.Write (": ");

            this.body.Debug (output);

            output.Write ("}");
        }

        #endregion
    }
}
