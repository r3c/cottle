using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle.Expressions.Generics
{
    abstract class  ConstantExpression<T> : IExpression
    {
        #region Attributes

        protected IValue    value;

        #endregion

        #region Constructors

        public  ConstantExpression (IValue value)
        {
            this.value = value;
        }

        #endregion

        #region Methods

        public IValue   Evaluate (Scope scope)
        {
            return this.value;
        }

        public abstract override string ToString ();

        #endregion
    }
}
