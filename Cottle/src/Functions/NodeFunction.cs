using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Values;

namespace   Cottle.Functions
{
    class   NodeFunction : IFunction
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

        public Value    Execute (IList<Value> values, Scope scope, TextWriter output)
        {
            Value   result;
            int     i = 0;

            scope.Enter ();

            foreach (NameExpression argument in this.arguments)
            {
                argument.Set (scope, i < values.Count ? values[i] : UndefinedValue.Instance, ScopeSet.Local);

                ++i;
            }

            this.body.Apply (scope, output, out result);

            scope.Leave ();

            return result;
        }

        #endregion
    }
}
