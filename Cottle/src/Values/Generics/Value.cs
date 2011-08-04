using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values.Generics
{
    using   ChildList = List<KeyValuePair<IValue, IValue>>;

    public abstract class   Value : IValue
    {
        #region Constants

        protected static readonly ChildList EmptyChildren = new List<KeyValuePair<IValue, IValue>> ();

        #endregion

        #region Properties

        public abstract bool        AsBoolean
        {
            get;
        }

        public abstract Function    AsFunction
        {
            get;
        }

        public abstract decimal     AsNumber
        {
            get;
        }

        public abstract string      AsString
        {
            get;
        }

        public abstract ChildList   Children
        {
            get;
        }

        #endregion

        #region Methods

        public abstract bool    Equals (IValue other);

        public abstract bool    Find (IValue key, out IValue value);

        public abstract override int    GetHashCode ();

        public abstract bool    Has (IValue key);

        public abstract override string ToString ();

        #endregion
    }
}
