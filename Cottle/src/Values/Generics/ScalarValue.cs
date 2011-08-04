using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values.Generics
{
    using   ChildList = List<KeyValuePair<IValue, IValue>>;

    public abstract class   ScalarValue<T> : Value
    {
        #region Properties

        public override ChildList   Children
        {
            get
            {
                return ScalarValue<T>.EmptyChildren;
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

        public override bool    Find (IValue key, out IValue value)
        {
            value = UndefinedValue.Instance;

            return false;
        }

        public override int GetHashCode ()
        {
            return this.value.GetHashCode ();
        }

        public override bool    Has (IValue key)
        {
            return false;
        }

        #endregion
    }
}
