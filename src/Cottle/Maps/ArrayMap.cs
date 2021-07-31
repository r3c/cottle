using System.Collections.Generic;
using System.Linq;

namespace Cottle.Maps
{
    internal class ArrayMap : AbstractMap
    {
        private readonly List<Value> _array;

        public ArrayMap(IEnumerable<Value> array)
        {
            _array = array.ToList();
        }

        public override int Count => _array.Count;

        public override bool Contains(Value key)
        {
            if (key.Type != ValueContent.Number)
                return false;

            var index = (int)key.AsNumber;

            return index >= 0 && index < _array.Count;
        }

        public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator()
        {
            for (var i = 0; i < _array.Count; ++i)
                yield return new KeyValuePair<Value, Value>(i, _array[i]);
        }

        public override bool TryGet(Value key, out Value value)
        {
            if (key.Type != ValueContent.Number)
            {
                value = default;

                return false;
            }

            var index = (int)key.AsNumber;

            if (index < 0 || index >= _array.Count)
            {
                value = default;

                return false;
            }

            value = _array[index];

            return true;
        }
    }
}