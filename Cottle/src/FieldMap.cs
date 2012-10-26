using System.Collections;
using System.Collections.Generic;

using Cottle.Values;

namespace   Cottle
{
    public sealed class FieldMap : IEnumerable<KeyValuePair<Value, Value>>
    {
        #region Properties / Instance

        public int  Count
        {
            get
            {
                return this.array.Count;
            }
        }

        #endregion

        #region Properties / Static

        public static FieldMap  Empty
        {
            get
            {
                return FieldMap.emptyMap;
            }
        }

        #endregion

        #region Attributes / Instance

        private IList<KeyValuePair<Value, Value>>   array;

        private Dictionary<Value, Value>            hash;

        #endregion

        #region Attributes / Static

        private static readonly KeyValuePair<Value, Value>[]    emptyArray = new KeyValuePair<Value, Value>[0];

        private static readonly Dictionary<Value, Value>        emptyHash = new Dictionary<Value, Value> ();

        private static readonly FieldMap                        emptyMap = new FieldMap ();

        #endregion

        #region Constructors

        public  FieldMap (IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            this.array = new List<KeyValuePair<Value, Value>> (pairs);
            this.hash = new Dictionary<Value, Value> ();

            foreach (KeyValuePair<Value, Value> pair in pairs)
                this.hash[pair.Key] = pair.Value;
        }

        public  FieldMap (IEnumerable<Value> values)
        {
            Value   key;
            int     i;

            this.array = new List<KeyValuePair<Value, Value>> ();
            this.hash = new Dictionary<Value, Value> ();

            i = 0;

            foreach (Value value in values)
            {
                key = new NumberValue (i++);

                this.array.Add (new KeyValuePair<Value, Value> (key, value));
                this.hash.Add (key, value);
            }
        }

        public  FieldMap ()
        {
            this.array = FieldMap.emptyArray;
            this.hash = FieldMap.emptyHash;
        }

        #endregion

        #region Methods
        
        public bool Contains (Value key)
        {
            return this.hash.ContainsKey (key);
        }

        public IEnumerator<KeyValuePair<Value, Value>>  GetEnumerator()
        {
            return this.array.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator ();
        }

        public override int GetHashCode ()
        {
            int hash = 0;

            foreach (KeyValuePair<Value, Value> item in this.array)
                hash = (hash << 1) ^ item.Key.GetHashCode () ^ item.Value.GetHashCode ();

            return hash;
        }

        public bool TryGet (Value key, out Value value)
        {
            return this.hash.TryGetValue (key, out value);
        }

        #endregion
    }
}
