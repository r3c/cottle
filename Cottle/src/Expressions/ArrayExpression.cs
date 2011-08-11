using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions.Generics;
using Cottle.Values;

namespace   Cottle.Expressions
{
    sealed class    ArrayExpression : Expression
    {
        #region Attributes

        private List<KeyValuePair<IExpression, IExpression>>    elements;

        #endregion

        #region Constructors

        public  ArrayExpression (IEnumerable<KeyValuePair<IExpression, IExpression>> elements)
        {
            this.elements = new List<KeyValuePair<IExpression,IExpression>> (elements);
        }

        #endregion

        #region Methods

        public override Value  Evaluate (Scope scope, TextWriter writer)
        {
            return new ArrayValue (this.elements.ConvertAll (delegate (KeyValuePair<IExpression, IExpression> item)
            {
                Value  key = item.Key.Evaluate (scope, writer);
                Value  value = item.Value.Evaluate (scope, writer);

                return new KeyValuePair<Value, Value> (key, value);
            }));
        }

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            comma = false;

            builder.Append ('[');

            foreach (KeyValuePair<IExpression, IExpression> element in this.elements)
            {
                if (comma)
                    builder.Append (", ");
                else
                    comma = true;

                builder.Append (element.Key);
                builder.Append (": ");
                builder.Append (element.Value);
            }

            builder.Append (']');

            return builder.ToString ();
        }

        #endregion
    }
}
