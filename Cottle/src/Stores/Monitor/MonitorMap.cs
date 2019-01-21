using System.Collections;
using System.Collections.Generic;
using Cottle.Stores.Monitor.SymbolUsages;
using Cottle.Values;

namespace Cottle.Stores.Monitor
{
    class MonitorMap : IMap
    {
        public int Count => this.map.Count;

        private readonly IMap map;
        private readonly MutableSymbolUsage usage;

        public MonitorMap(IMap map, MutableSymbolUsage usage)
        {
            this.map = map;
            this.usage = usage;
        }

        public int CompareTo(IMap other)
        {
            return this.map.CompareTo(other);
        }

        public IEnumerator<KeyValuePair<Value, Value>> GetEnumerator()
        {
            foreach (KeyValuePair<Value, Value> pair in this.map)
            {
                var child = this.usage.Declare(pair.Key, pair.Value);
                var value = new MonitorValue(pair.Value, child);

                yield return new KeyValuePair<Value, Value>(pair.Key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool Equals(IMap other)
        {
            return this.map.Equals(other);
        }

        public bool Contains(Value key)
        {
            return this.map.Contains(key);
        }

        public bool TryGet(Value key, out Value value)
        {
            var result = this.map.TryGet(key, out Value original);

            if (!result)
                original = VoidValue.Instance;

            var child = this.usage.Declare(key, original);

            value = new MonitorValue(original, child);

            return result;
        }
    }
}
