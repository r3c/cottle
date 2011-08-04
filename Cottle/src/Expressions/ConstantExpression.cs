using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Expressions.Generics;

namespace   Cottle.Expressions
{
    sealed class    ConstantExpression : Expression
    {
        #region Attributes

        private IValue  value;

        #endregion

        #region Constructors

        public  ConstantExpression (IValue value)
        {
            this.value = value;
        }

        #endregion

        #region Methods

        public override IValue  Evaluate (Scope scope)
        {
            return this.value;
        }

        public override string  ToString ()
        {
            return this.value.ToString ();
        }

        #endregion
    }
}
