using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class ArrayValue : Value
    {
        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return this.list.Count > 0;
            }
        }

        public override Function    AsFunction
        {
            get
            {
                return Function.Undefined;
            }
        }

        public override decimal     AsNumber
        {
            get
            {
                return this.list.Count;
            }
        }

        public override string      AsString
        {
            get
            {
                return "<array>";
            }
        }

        #endregion

        #region Constructors

        public  ArrayValue (IEnumerable<KeyValuePair<IValue, IValue>> collection) :
            base (collection)
        {
        }

        public  ArrayValue ()
        {
        }

        #endregion

        #region Methods

        public override string  ToString ()
        {
            StringBuilder   builder = new StringBuilder ();
            bool            separator = false;

            builder.Append ('[');

            foreach (KeyValuePair<IValue, IValue> item in this.list)
            {
                if (separator)
                    builder.Append (", ");
                else
                    separator = true;

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
