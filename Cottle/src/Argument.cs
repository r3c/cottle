using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace   Cottle
{
    public struct   Argument
    {
        #region Properties
        
        public Value   Value
        {
            get
            {
                return this.expression.Evaluate (this.scope, this.output);
            }
        }

        #endregion

        #region Attributes

        private IExpression expression;

        private TextWriter  output;

        private Scope       scope;

        #endregion

        #region Constructors

        internal    Argument (Scope scope, IExpression expression, TextWriter output)
        {
            this.expression = expression;
            this.output = output;
            this.scope = scope;
        }

        #endregion
    }
}
