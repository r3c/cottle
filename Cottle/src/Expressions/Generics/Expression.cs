using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle.Expressions.Generics
{
    abstract class  Expression : IExpression
    {
        #region Methods

        public abstract Value           Evaluate (Scope scope, TextWriter output);

        public abstract override string ToString ();

        #endregion
    }
}
