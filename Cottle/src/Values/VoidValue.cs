using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Values.Generics;

namespace   Cottle.Values
{
    using   FieldList = List<KeyValuePair<Value, Value>>;

    public sealed class VoidValue : Value
    {
        #region Constants

        public static readonly VoidValue    Instance = new VoidValue ();

        #endregion

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

        public override FieldList   Fields
        {
            get
            {
                return Value.EmptyFields;
            }
        }

        public override DataType    Type
        {
            get
            {
                return DataType.VOID;
            }
        }

        #endregion

        #region Methods

        public override bool    Find (Value key, out Value value)
        {
            value = VoidValue.Instance;

            return false;
        }

        public override bool    Has (Value key)
        {
            return false;
        }

        public override string  ToString ()
        {
            return "<void>";
        }

        #endregion
    }
}
