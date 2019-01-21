using System.Collections.Generic;

namespace Cottle.Maps
{
    internal class MixMap : AbstractMap
    {
        #region Constructors

        public MixMap(IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            _array = new List<KeyValuePair<Value, Value>>(pairs);
            _hash = new Dictionary<Value, Value>();

            foreach (var pair in _array)
                _hash[pair.Key] = pair.Value;
        }

        #endregion

        #region Properties

        public override int Count => _array.Count;

        #endregion

        #region Attributes

        private readonly List<KeyValuePair<Value, Value>> _array;

        private readonly Dictionary<Value, Value> _hash;

        #endregion

        #region Methods

        public override bool Contains(Value key)
        {
            return _hash.ContainsKey(key);
        }

        public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        public override bool TryGet(Value key, out Value value)
        {
            return _hash.TryGetValue(key, out value);
        }

        #endregion
    }
}