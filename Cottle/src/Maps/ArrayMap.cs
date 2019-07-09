using System.Collections.Generic;

namespace Cottle.Maps
{
    internal class ArrayMap : AbstractMap
    {
        private readonly List<KeyValuePair<Value, Value>> _array;

        public ArrayMap(IEnumerable<Value> array)
        {
            _array = new List<KeyValuePair<Value, Value>>();

            var key = 0;

            foreach (var value in array)
                _array.Add(new KeyValuePair<Value, Value>(key++, value));
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
            return _array.GetEnumerator();
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

            value = _array[index].Value;

            return true;
        }
    }
}