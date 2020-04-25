using System.Collections;
using System.Collections.Generic;

namespace Cottle.Maps
{
    internal abstract class AbstractMap : IMap
    {
        public Value this[Value key] => TryGet(key, out var value) ? value : Value.Undefined;

        public abstract int Count { get; }

        public abstract bool Contains(Value key);

        public abstract IEnumerator<KeyValuePair<Value, Value>> GetEnumerator();

        public abstract bool TryGet(Value key, out Value value);

        public int CompareTo(IMap other)
        {
            if (other == null)
                return 1;

            if (Count < other.Count)
                return -1;
            if (Count > other.Count)
                return 1;

            using (var lhs = GetEnumerator())
            {
                using (var rhs = other.GetEnumerator())
                {
                    while (lhs.MoveNext() && rhs.MoveNext())
                    {
                        var compare = lhs.Current.Key.CompareTo(rhs.Current.Key);

                        if (compare != 0)
                            return compare;

                        compare = lhs.Current.Value.CompareTo(rhs.Current.Value);

                        if (compare != 0)
                            return compare;
                    }
                }
            }

            return 0;
        }

        public bool Equals(IMap other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is IMap other && Equals(other);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            var hash = 0;

            foreach (var item in this)
                hash = (hash << 1) ^ item.Key.GetHashCode() ^ item.Value.GetHashCode();

            return hash;
        }
    }
}