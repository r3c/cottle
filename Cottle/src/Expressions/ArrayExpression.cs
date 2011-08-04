using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Expressions.Generics;
using Cottle.Values;

namespace   Cottle.Expressions
{
    sealed class    ArrayExpression : Expression
    {
        #region Attributes

        private List<KeyValuePair<IExpression, IExpression>>    list;

        #endregion

        #region Constructors

        public  ArrayExpression (IEnumerable<KeyValuePair<IExpression, IExpression>> list)
        {
            this.list = new List<KeyValuePair<IExpression,IExpression>> (list);
        }

        #endregion

        #region Methods

        public override IValue  Evaluate (Scope scope)
        {
            return new ArrayValue (this.list.ConvertAll (delegate (KeyValuePair<IExpression, IExpression> item)
            {
                return new KeyValuePair<IValue, IValue> (item.Key.Evaluate (scope), item.Value.Evaluate (scope));
            }));
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            comma = false;

            builder.Append ('[');

            foreach (KeyValuePair<IExpression, IExpression> item in this.list)
            {
                if (comma)
                    builder.Append (", ");
                else
                    comma = true;

                builder.Append (item.Key);
                builder.Append (": ");
                builder.Append (item.Value);
            }

            builder.Append (']');

            return builder.ToString ();
        }

        #endregion
    }
}
