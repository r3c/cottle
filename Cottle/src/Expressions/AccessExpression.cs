using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Expressions.Generics;
using Cottle.Values;

namespace   Cottle.Expressions
{
    class   AccessExpression : Expression
    {
        #region Attributes

        private IExpression array;

        private IExpression index;

        #endregion

        #region Constructors

        public  AccessExpression (IExpression array, IExpression index)
        {
            this.array = array;
            this.index = index;
        }

        #endregion

        #region Methods

        public override IValue  Evaluate (Scope scope)
        {
            IValue  value;

            if (this.array.Evaluate (scope).Find (this.index.Evaluate (scope), out value))
                return value;

            return UndefinedValue.Instance;
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();

            builder.Append (this.array);
            builder.Append ('[');
            builder.Append (this.index);
            builder.Append (']');

            return builder.ToString ();
        }

        #endregion
    }
}
