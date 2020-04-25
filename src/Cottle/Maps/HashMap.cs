using System.Collections.Generic;

namespace Cottle.Maps
{
    internal class HashMap : AbstractMap
    {
        private readonly IReadOnlyDictionary<Value, Value> _hash;

        public HashMap(IReadOnlyDictionary<Value, Value> hash)
        {
            _hash = hash;
        }

        public HashMap(IDictionary<Value, Value> hash)
        {
            _hash = new Dictionary<Value, Value>(hash);
        }

        public override int Count => _hash.Count;

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
    }
}