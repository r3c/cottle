using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Values;

namespace   Cottle.Functions
{
    class   CallbackFunction : Function
    {
        #region Attributes

        private CallbackDelegate    callback;

        private int                 max;

        private int                 min;

        #endregion

        #region Constructors

        public  CallbackFunction (CallbackDelegate callback, int min, int max)
        {
            this.callback = callback;
            this.max = max;
            this.min = min;
        }

        public  CallbackFunction (CallbackDelegate callback, int exact) :
            this (callback, exact, exact)
        {
        }

        public  CallbackFunction (CallbackDelegate callback) :
            this (callback, 0, -1)
        {
        }

        #endregion

        #region Methods

        internal override Value    Execute (Scope scope, IList<IExpression> expressions, TextWriter output)
        {
            Argument[]  arguments;
            int         i;

            if (this.callback == null || this.min > expressions.Count || (this.max >= 0 && this.max < expressions.Count))
                return UndefinedValue.Instance;

            arguments = new Argument[expressions.Count];

            for (i = expressions.Count; i-- > 0; )
                arguments[i] = new Argument (scope, expressions[i], output);

            return this.callback (arguments);
        }

        #endregion

        #region Types

        public delegate Value  CallbackDelegate (Argument[] arguments);

        #endregion
    }
}
