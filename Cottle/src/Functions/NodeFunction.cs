using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Values;

namespace   Cottle.Functions
{
    class   NodeFunction : Function
    {
        #region Attributes

        private List<NameExpression>    arguments;

        private INode                   body;

        #endregion

        #region Constructors

        public  NodeFunction (IEnumerable<NameExpression> arguments, INode body)
        {
            this.arguments = new List<NameExpression> (arguments);
            this.body = body;
        }

        #endregion

        #region Methods

        internal override Value    Execute (Scope scope, IList<IExpression> expressions, TextWriter output)
        {
            Value  result;
            int     i = 0;

            scope.Enter ();

            foreach (NameExpression argument in this.arguments)
            {
                if (i < expressions.Count)
                    argument.Set (scope, expressions[i++].Evaluate (scope, output), Scope.SetMode.LOCAL);
                else
                    argument.Set (scope, UndefinedValue.Instance, Scope.SetMode.LOCAL);
            }

            this.body.Apply (scope, output, out result);

            scope.Leave ();

            return result;
        }

        #endregion
    }
}
