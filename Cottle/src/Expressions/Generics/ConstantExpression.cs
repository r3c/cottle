using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle.Expressions.Generics
{
    abstract class  ConstantExpression<T> : Expression
    {
        #region Attributes

        private IValue  value;

        #endregion

        #region Constructors

        protected  ConstantExpression (IValue value)
        {
            this.value = value;
        }

        #endregion

        #region Methods

        public override IValue  Evaluate (Scope scope, TextWriter output)
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
