using System.Collections;
using System.Collections.Generic;
using Cottle.Contexts.Monitor.SymbolUsages;
using Cottle.Evaluables;

namespace Cottle.Contexts.Monitor
{
    internal class MonitorMap : IMap
    {
        private readonly IMap _map;
        private readonly MutableSymbolUsage _usage;

        public MonitorMap(IMap map, MutableSymbolUsage usage)
        {
            _map = map;
            _usage = usage;
        }

        public Value this[Value key]
        {
            get
            {
                // Output value is always defined by this implementation, see
                // method `TryGet` for details.
                TryGet(key, out var value);

                return value;
            }
        }

        public int Count => _map.Count;

        public int CompareTo(IMap? other)
        {
            return other is not null ? _map.CompareTo(other) : 1;
        }

        public IEnumerator<KeyValuePair<Value, Value>> GetEnumerator()
        {
            foreach (var pair in _map)
            {
                var child = _usage.Declare(pair.Key, pair.Value);
                var value = Value.FromEvaluable(new MonitorEvaluable(pair.Value, child));

                yield return new KeyValuePair<Value, Value>(pair.Key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(IMap? other)
        {
            return other is not null && _map.Equals(other);
        }

        public bool Contains(Value key)
        {
            return _map.Contains(key);
        }

        public bool TryGet(Value key, out Value value)
        {
            var result = _map.TryGet(key, out var original);

            if (!result)
                original = Value.Undefined;

            var child = _usage.Declare(key, original);

            value = Value.FromEvaluable(new MonitorEvaluable(original, child));

            return result;
        }
    }
}