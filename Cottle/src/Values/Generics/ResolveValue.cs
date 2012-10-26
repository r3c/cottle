using System.Threading;

namespace   Cottle.Values.Generics
{
    public abstract class   ResolveValue : Value
    {
        #region Properties

        public override bool            AsBoolean
        {
            get
            {
                return this.Acquire ().AsBoolean;
            }
        }

        public override IFunction       AsFunction
        {
            get
            {
                return this.Acquire ().AsFunction;
            }
        }

        public override decimal         AsNumber
        {
            get
            {
                return this.Acquire ().AsNumber;
            }
        }

        public override string          AsString
        {
            get
            {
                return this.Acquire ().AsString;
            }
        }

        public override FieldMap        Fields
        {
            get
            {
                return this.Acquire ().Fields;
            }
        }

        public override ValueContent    Type
        {
            get
            {
                return this.Acquire ().Type;
            }
        }

        #endregion

        #region Attributes

        private object  synchronize = new object ();

        private Value   value = null;

        #endregion

        #region Methods / Abstract

        protected abstract Value    Resolve ();

        #endregion

        #region Methods / Public

        public override int CompareTo (Value other)
        {
            return this.Acquire ().CompareTo (other);
        }

        public override int GetHashCode ()
        {
            return this.Acquire ().GetHashCode ();
        }

        public override string  ToString ()
        {
        	return this.Acquire ().ToString ();
        }

        #endregion

        #region Methods / Private

        private Value   Acquire ()
        {
            if (this.value == null)
            {
                lock (this.synchronize)
                {
                    if (this.value == null)
                        Interlocked.Exchange (ref this.value, this.Resolve ());
                }
            }

            return this.value;
        }

        #endregion
    }
}
