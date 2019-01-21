using System.Collections.Generic;

namespace Cottle.Maps
{
    internal class HashMap : AbstractMap
    {
        #region Attributes

        private readonly Dictionary<Value, Value> _hash;

        #endregion

        #region Constructors

        public HashMap(IDictionary<Value, Value> hash)
        {
            _hash = new Dictionary<Value, Value>(hash);
        }

        #endregion

        #region Properties

        public override int Count => _hash.Count;

        #endregion

        #region Methods

        public override bool Contains(Value key)
        {
            return _hash.ContainsKey(key);
        }

        public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator()
        {
            return _hash.GetEnumerator();
        }

        public override bool TryGet(Value key, out Value value)
        {
            return _hash.TryGetValue(key, out value);
        }

        #endregion
    }
}