using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values.Generics
{
    public abstract class   EmptyValue : IValue
    {
        #region Properties

        public abstract bool                AsBoolean
        {
            get;
        }

        public abstract decimal             AsNumber
        {
            get;
        }

        public abstract string              AsString
        {
            get;
        }

        public IDictionary<string, IValue>  Children
        {
            get
            {
                return EmptyValue.empty;
            }
        }

        #endregion

        #region Attributes

        private static IDictionary<string, IValue>  empty = new Dictionary<string, IValue> (0);

        #endregion

        #region Methods

        public abstract override string ToString ();

        #endregion
    }
}
