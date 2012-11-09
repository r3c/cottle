using System.Collections;
using System.Collections.Generic;

using Cottle.Maps.Generics;
using Cottle.Values;

namespace   Cottle.Maps
{
    sealed class    MixMap : AbstractMap
    {
        #region Properties

        public override int Count
        {
            get
            {
                return this.array.Count;
            }
        }

        #endregion

        #region Attributes

        private List<KeyValuePair<Value, Value>>    array;

        private Dictionary<Value, Value>            hash;

        #endregion

        #region Constructors

        public  MixMap (IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            this.array = new List<KeyValuePair<Value, Value>> (pairs);
            this.hash = new Dictionary<Value, Value> ();

            foreach (KeyValuePair<Value, Value> pair in pairs)
                this.hash[pair.Key] = pair.Value;
        }

        #endregion

        #region Methods
        
        public override bool   Contains (Value key)
        {
            return this.hash.ContainsKey (key);
        }

        public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator ()
        {
            return this.array.GetEnumerator ();
        }

        public override bool   TryGet (Value key, out Value value)
        {
            return this.hash.TryGetValue (key, out value);
        }

        #endregion
    }
}
