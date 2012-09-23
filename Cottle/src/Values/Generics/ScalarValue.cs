using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values.Generics
{
    public abstract class   ScalarValue<T> : Value
    {
        #region Properties

        public override KeyValuePair<Value, Value>[]    Fields
        {
            get
            {
                return Value.emptyFields;
            }
        }

        #endregion

        #region Attributes

        protected Comparer<T>   comparer;

        protected T             value;

        #endregion

        #region Constructors

        protected   ScalarValue (T value)
        {
            this.comparer = Comparer<T>.Default;
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
