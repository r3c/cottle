using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        public override Value  Evaluate (Scope scope, TextWriter output)
        {
            Function   function = this.caller.Evaluate (scope, output).AsFunction;

            if (function != null)
                return function.Execute (scope, this.arguments, output);

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
