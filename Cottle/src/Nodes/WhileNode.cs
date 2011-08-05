using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    WhileNode : INode
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

        public bool Apply (Scope scope, TextWriter output, out Value result)
        {
            while (this.test.Evaluate (scope, output).AsBoolean)
            {
                if (this.body.Apply (scope, output, out result))
                    return true;
            }

            result = UndefinedValue.Instance;

            return false;
        }

        public void Debug (TextWriter output)
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
