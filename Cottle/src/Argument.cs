using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle
{
    public struct   Argument
    {
        #region Properties
        
        public IValue   Value
        {
            get
            {
                return this.expression.Evaluate (this.scope);
            }
        }

        #endregion

        #region Attributes

        private IExpression expression;

        private Scope       scope;

        #endregion

        #region Constructors

        internal    Argument (Scope scope, IExpression expression)
        {
            this.expression = expression;
            this.scope = scope;
        }

        #endregion
    }
}
