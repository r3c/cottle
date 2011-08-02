using System;
using System.Collections.Generic;
using System.Text;

using Cottle;
using Cottle.Values;

namespace   Cottle.Expressions
{
    class   VarExpression : IExpression
    {
        #region Attributes

        private static readonly IValue  empty = new DefaultValue ();

        private string                  name;

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

            if (value.Children.TryGetValue (this.name, out child))
                return child;

            return VarExpression.empty;
        }

        public IValue   Evaluate (Scope scope)
        {
            IValue  value;

            if (scope.Get (name, out value))
                return value;

            return VarExpression.empty;
        }

        public void Set (Scope scope, IValue value)
        {
            scope.Set (this.name, value);
        }

        public override string  ToString ()
        {
            return this.name;
        }

        #endregion
    }
}
