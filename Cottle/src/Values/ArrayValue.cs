using System;
using System.Collections.Generic;
using System.Text;

namespace   Cottle.Values
{
    [Obsolete("Please use MapValue instead")]
    public sealed class ArrayValue : MapValue
    {
        #region Constructors
        
        public  ArrayValue (IDictionary<Value, Value> hash) :
            base (hash)
        {
        }

        public  ArrayValue (IEnumerable<KeyValuePair<Value, Value>> pairs) :
            base (pairs)
        {
        }

        public  ArrayValue (IEnumerable<Value> values) :
            base (values)
        {
        }

        public  ArrayValue ()
        {
        }

        #endregion
    }
}
