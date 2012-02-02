using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    public sealed class UndefinedValue : Value
    {
        #region Properties

        public override bool        AsBoolean
        {
            get
            {
                return false;
            }
        }

        public override IFunction   AsFunction
        {
            get
            {
                return null;
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
                return string.Empty;
            }
        }

        public override KeyValuePair<Value, Value>[]    Fields
        {
            get
            {
                return Value.EmptyFields;
            }
        }

        public override ValueContent    Type
        {
            get
            {
                return ValueContent.Undefined;
            }
        }

        #endregion

        #region Attributes

        public static readonly UndefinedValue   Instance = new UndefinedValue ();

        #endregion

        #region Methods

        public override int CompareTo (Value other)
        {
            if (other == null)
                return 1;
            else if (other.Type != ValueContent.Undefined)
                return -1;
            else
                return 0;
        }

        public override bool    Find (Value key, out Value value)
        {
            value = UndefinedValue.Instance;

            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool    Has (Value key)
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
