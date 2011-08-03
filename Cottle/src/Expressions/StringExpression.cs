using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Cottle.Expressions.Generics;
using Cottle.Values;

namespace   Cottle.Expressions
{
    class   StringExpression : ConstantExpression<string>
    {
        #region Constructors

        public  StringExpression (string constant) :
            base (new StringValue (constant))
        {
        }

        #endregion

        #region Methods

        public override string  ToString ()
        {
            return string.Format ("\"{0}\"", this.value.AsString);
        }

        #endregion
    }
}
