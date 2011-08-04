using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle.Expressions.Generics
{
    abstract class  Expression : IExpression
    {
        #region Methods

        public abstract IValue          Evaluate (Scope scope);

        public abstract override string ToString ();

        #endregion
    }
}
