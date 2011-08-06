using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values.Generics
{
    using   FieldList = List<KeyValuePair<Value, Value>>;

    public abstract class   ScalarValue<T> : Value
    {
        #region Properties

        public override FieldList   Fields
        {
            get
            {
                return Value.EmptyFields;
            }
        }

        #endregion

        #region Attributes

        protected T value;

        #endregion

        #region Constructors

        protected   ScalarValue (T value)
        {
            this.value = value;
        }

        #endregion

        #region Methods

        public override bool    Find (Value key, out Value value)
        {
            value = UndefinedValue.Instance;

            return false;
        }

        public override int GetHashCode ()
        {
            return this.value.GetHashCode ();
        }

        public override bool    Has (Value key)
        {
            return false;
        }

        #endregion
    }
}
