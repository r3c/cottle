using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values;

namespace   Cottle.Expressions
{
    class   FunctionExpression : IExpression
    {
        #region Attributes

        private List<IExpression>   arguments;

        private VarExpression       caller;

        #endregion

        #region Constructors

        public  FunctionExpression (VarExpression caller, IEnumerable<IExpression> arguments)
        {
            this.arguments = new List<IExpression> (arguments);
            this.caller = caller;
        }

        #endregion

        #region Methods

        public IValue   Evaluate (Scope scope)
        {
            Argument[]  arguments;
            Function    function;
            int         i;

            function = this.caller.Evaluate (scope).AsFunction;

            if ((function.Callback != null) &&
                (function.Min <= this.arguments.Count) &&
                (function.Max < 0 || function.Max >= this.arguments.Count))
            {
                arguments = new Argument[this.arguments.Count];

                for (i = this.arguments.Count; i-- > 0; )
                    arguments[i] = new Argument (scope, this.arguments[i]);

                return function.Callback (arguments);
            }

            return UndefinedValue.Instance;
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            dot = false;

            builder.Append (this.caller);
            builder.Append (" (");

            foreach (IExpression argument in this.arguments)
            {
                if (dot)
                    builder.Append (", ");
                else
                    dot = true;

                builder.Append (argument);
            }

            builder.Append (")");

            return builder.ToString ();
        }

        #endregion
    }
}
