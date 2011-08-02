using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values;

namespace   Cottle.Expressions
{
    class   AccessExpression : IExpression
    {
        #region Attributes

        private static readonly IValue      empty = new DefaultValue ();

        private IEnumerable<VarExpression>  fields;

        #endregion

        #region Constructors

        public  AccessExpression (IEnumerable<VarExpression> fields)
        {
            this.fields = fields;
        }

        #endregion

        #region Methods

        public IValue   Evaluate (Scope scope)
        {
            IValue  value = null;

            foreach (VarExpression field in this.fields)
            {
                if (value != null)
                    value = field.Dereference (value);
                else
                    value = field.Evaluate (scope);
            }

            return value != null ? value : AccessExpression.empty;
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            dot = false;

            foreach (VarExpression field in this.fields)
            {
                if (dot)
                    builder.Append ('.');
                else
                    dot = true;

                builder.Append (field.ToString ());
            }

            return builder.ToString ();
        }

        #endregion
    }
}
