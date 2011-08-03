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
    }
}
