using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    using   ChildList = List<KeyValuePair<IValue, IValue>>;

    public sealed class UndefinedValue : Value
    {
        #region Constants

        public static readonly UndefinedValue   Instance = new UndefinedValue ();

        #endregion

        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return false;
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
                return 0;
            }
        }

        public override string      AsString
        {
            get
            {
                return "<undefined>";
            }
        }

        public override ChildList   Children
        {
            get
            {
                return Value.EmptyChildren;
            }
        }

        #endregion

        #region Methods

        public override bool    Equals (IValue other)
        {
            return false;
        }

        public override bool    Find (IValue key, out IValue value)
        {
            value = UndefinedValue.Instance;

            return false;
        }

        public override int GetHashCode ()
        {
            return 0;
        }

        public override bool    Has (IValue key)
        {
            return false;
        }

        public override string  ToString ()
        {
            return "<undefined>";
        }

        #endregion
    }
}
