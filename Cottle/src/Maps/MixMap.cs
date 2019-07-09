using System.Collections.Generic;

namespace Cottle.Maps
{
    internal class MixMap : AbstractMap
    {
        public override int Count => _array.Count;

        private readonly List<KeyValuePair<Value, Value>> _array;

        private readonly Dictionary<Value, Value> _hash;

        public MixMap(IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            _array = new List<KeyValuePair<Value, Value>>(pairs);
            _hash = new Dictionary<Value, Value>();

            foreach (var pair in _array)
                _hash[pair.Key] = pair.Value;
        }

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
    }
}