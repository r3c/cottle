using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Expressions
{
    class   FunctionExpression : IExpression
    {
        #region Attributes

        private IEnumerable<IExpression>    arguments;

        private Function                    function;

        #endregion

        #region Constructors

        public  FunctionExpression (Function function, IEnumerable<IExpression> arguments)
        {
            this.arguments = arguments;
            this.function = function;
        }

        #endregion

        #region Methods

        public IValue   Evaluate (Scope scope)
        {
            List<IValue>    values = new List<IValue> ();

            foreach (IExpression argument in this.arguments)
                values.Add (argument.Evaluate (scope));

            return this.function (scope, values);
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            dot = false;

            builder.Append ("<fun> (");

            foreach (IExpression argument in this.arguments)
            {
                if (dot)
                    builder.Append (", ");
                else
                    dot = true;

                builder.Append (argument.ToString ());
            }

            builder.Append (")");

            return builder.ToString ();
        }

        #endregion
    }
}
