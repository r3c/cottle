using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Expressions.Generics;
using Cottle.Values;

namespace   Cottle.Expressions
{
    sealed class    NumberExpression : ConstantExpression<decimal>
    {
        #region Constructors

        public  NumberExpression (decimal value) :
            base (new NumberValue (value))
        {
        }

        #endregion
    }
}
