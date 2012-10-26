using System.Collections.Generic;

namespace   Cottle.Values.Generics
{
    public abstract class   ScalarValue<T> : Value
    {
        #region Properties

        public override FieldMap    Fields
        {
            get
            {
                return FieldMap.Empty;
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

        public override int GetHashCode ()
        {
            return this.value.GetHashCode ();
        }

        #endregion
    }
}
