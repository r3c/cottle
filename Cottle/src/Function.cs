using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Values;

namespace   Cottle
{
    public struct   Function
    {
        #region Constants

        public static readonly Function Undefined = new Function (null);

        #endregion

        #region Attributes

        private List<NameExpression>    arguments;

        private INode                   body;

        private CallbackDelegate        callback;

        private int                     max;

        private int                     min;

        #endregion

        #region Constructors

        public      Function (CallbackDelegate callback, int min, int max)
        {
            this.arguments = null;
            this.body = null;
            this.callback = callback;
            this.max = max;
            this.min = min;
        }

        public      Function (CallbackDelegate callback, int exact) :
            this (callback, exact, exact)
        {
        }

        public      Function (CallbackDelegate callback) :
            this (callback, 0, -1)
        {
        }

        internal    Function (IEnumerable<NameExpression> arguments, INode body)
        {
            this.arguments = new List<NameExpression> (arguments);
            this.body = body;
            this.callback = null;
            this.max = this.arguments.Count;
            this.min = 0;
        }

        #endregion

        #region Methods

        internal IValue Execute (Scope scope, IList<IExpression> expressions, TextWriter output)
        {
            Argument[]  arguments;
            IValue      result;
            int         i;

            if (this.min > expressions.Count || (this.max >= 0 && this.max < expressions.Count))
                return null;

            if (this.arguments != null && this.body != null)
            {
                scope.Enter ();

                i = 0;

                foreach (NameExpression argument in this.arguments)
                {
                    if (i < expressions.Count)
                        argument.Set (scope, expressions[i++].Evaluate (scope, output), Scope.SetMode.LOCAL);
                    else
                        argument.Set (scope, UndefinedValue.Instance, Scope.SetMode.LOCAL);
                }

                result = this.body.Apply (scope, output);

                scope.Leave ();
            }
            else if (this.callback != null)
            {
                arguments = new Argument[expressions.Count];

                for (i = expressions.Count; i-- > 0; )
                    arguments[i] = new Argument (scope, expressions[i], output);

                result = this.callback (arguments);
            }
            else
                result = null;

            return result;
        }

        #endregion

        #region Types

        public delegate IValue  CallbackDelegate (Argument[] arguments);

        #endregion
    }
}
