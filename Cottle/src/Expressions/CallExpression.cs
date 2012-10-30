using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Exceptions;
using Cottle.Expressions.Generics;
using Cottle.Values;

namespace   Cottle.Expressions
{
    sealed class    CallExpression : Expression
    {
        #region Attributes

        private List<IExpression>   arguments;

        private IExpression         caller;

        #endregion

        #region Constructors

        public  CallExpression (IExpression caller, IEnumerable<IExpression> arguments)
        {
            this.arguments = new List<IExpression> (arguments);
            this.caller = caller;
        }

        #endregion

        #region Methods

        public override Value   Evaluate (Scope scope, TextWriter output)
        {
            IFunction   function = this.caller.Evaluate (scope, output).AsFunction;
            Value[]     values = new Value[this.arguments.Count];
            int         i = 0;

            if (function != null)
            {
                foreach (IExpression argument in this.arguments)
                    values[i++] = argument.Evaluate (scope, output);

                try
                {
                    return function.Execute (values, scope, output);
                }
                catch (Exception exception)
                {
                    throw new RenderException ("function call raised an exception", exception);
                }
            }

            return UndefinedValue.Instance;
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            dot = false;

            builder.Append (this.caller);
            builder.Append ('(');

            foreach (IExpression argument in this.arguments)
            {
                if (dot)
                    builder.Append (", ");
                else
                    dot = true;

                builder.Append (argument);
            }

            builder.Append (')');

            return builder.ToString ();
        }

        #endregion
    }
}
