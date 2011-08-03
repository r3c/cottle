using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Cottle.Expressions.Generics;
using Cottle.Values;

namespace   Cottle.Expressions
{
    class   NumberExpression : ConstantExpression<decimal>
    {
        #region Constructors

        public  NumberExpression (decimal constant) :
            base (new NumberValue (constant))
        {
        }

        #endregion

        #region Methods

        public override string  ToString ()
        {
            return this.value.AsString;
        }

        #endregion
    }
}
