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

        private IEnumerable<NameExpression>  fields;

        #endregion

        #region Constructors

        public  AccessExpression (IEnumerable<NameExpression> fields)
        {
            this.fields = fields;
        }

        #endregion

        #region Methods

        public override IValue  Evaluate (Scope scope)
        {
            IValue  value = null;

            foreach (NameExpression field in this.fields)
            {
                if (value != null)
                    value = field.Dereference (value);
                else
                    value = field.Evaluate (scope);
            }

            return value != null ? value : UndefinedValue.Instance;
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            dot = false;

            foreach (NameExpression field in this.fields)
            {
                if (dot)
                    builder.Append ('/');
                else
                    dot = true;

                builder.Append (field);
            }

            return builder.ToString ();
        }

        #endregion
    }
}
