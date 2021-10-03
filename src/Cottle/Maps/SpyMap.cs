using System.Collections;
using System.Collections.Generic;
using Cottle.SpyRecords.Evaluable;

namespace Cottle.Maps
{
    internal sealed class SpyMap : IMap
    {
        public int Count => _map.Count;

        private readonly SpyLookup _lookup;

        private IMap _map;

        public SpyMap(IMap map)
        {
            _lookup = new SpyLookup();
            _map = map;
        }

        public Value this[Value key]
        {
            get
            {
                var initial = _map[key];
                var record = _lookup.CreateOrUpdate(key, initial);

                return Value.FromEvaluable(record);
            }
        }

        public int CompareTo(IMap? other)
        {
            return other is not null ? _map.CompareTo(other) : 1;
        }

        public bool Contains(Value key)
        {
            return _map.Contains(key);
        }

        public bool Equals(IMap? other)
        {
            return other is not null && _map.Equals(other);
        }

        public IEnumerator<KeyValuePair<Value, Value>> GetEnumerator()
        {
            foreach (var pair in _map)
            {
                var record = _lookup.GetOrCreate(pair.Key);
                var value = Value.FromEvaluable(record);

                yield return new KeyValuePair<Value, Value>(pair.Key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ISpyRecord SpyField(Value key)
        {
            return _lookup.GetOrCreate(key);
        }

        public IReadOnlyDictionary<Value, ISpyRecord> SpyFields()
        {
            var fields = new Dictionary<Value, ISpyRecord>(_map.Count);

            foreach (var key in _lookup.Keys)
                fields[key] = _lookup.GetOrCreate(key);

            return fields;
        }

        public bool TryGet(Value key, out Value value)
        {
            var result = _map.TryGet(key, out var initial);

            if (!result)
                initial = Value.Undefined;

            var record = _lookup.CreateOrUpdate(key, initial);

            value = Value.FromEvaluable(record);

            return result;
        }

        public void Update(IMap map)
        {
            _map = map;
        }
    }
}