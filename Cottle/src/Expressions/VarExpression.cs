using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Exceptions;
using Cottle.Values;

namespace   Cottle.Expressions
{
    class   VarExpression : IExpression
    {
        #region Attributes

        private string  name;

        #endregion

        #region Constructors

        public  VarExpression (string name)
        {
            this.name = name;
        }

        #endregion

        #region Methods

        public IValue   Dereference (IValue value)
        {
            IValue  child;

            if (value.Find (this.name, out child))
                return child;

            return UndefinedValue.Instance;
        }

        public IValue   Evaluate (Scope scope)
        {
            IValue  value;

            if (scope.Get (name, out value))
                return value;

            return UndefinedValue.Instance;
        }

        public bool Set (Scope scope, IValue value, Scope.SetMode mode)
        {
            return scope.Set (this.name, value, mode);
        }

        public override string  ToString ()
        {
            return this.name;
        }

        #endregion
    }
}
