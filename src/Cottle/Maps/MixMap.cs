using System.Collections.Generic;

namespace Cottle.Maps
{
    internal class MixMap : AbstractMap
    {
        public override int Count => _array.Count;

        private readonly IReadOnlyList<KeyValuePair<Value, Value>> _array;

        private readonly IReadOnlyDictionary<Value, Value> _hash;

        public MixMap(IEnumerable<KeyValuePair<Value, Value>> pairs)
        {
            var array = new List<KeyValuePair<Value, Value>>(pairs);
            var hash = new Dictionary<Value, Value>();

            foreach (var pair in array)
                hash[pair.Key] = pair.Value;

            _array = array;
            _hash = hash;
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