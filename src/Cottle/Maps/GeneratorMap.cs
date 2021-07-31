using System;
using System.Collections.Generic;

namespace Cottle.Maps
{
    internal class GeneratorMap : AbstractMap
    {
        public GeneratorMap(Func<int, Value> generator, int count)
        {
            _count = count;
            _generator = generator;
        }

        public override int Count => _count;

        private readonly int _count;

        private readonly Func<int, Value> _generator;

        public override bool Contains(Value key)
        {
            if (key.Type != ValueContent.Number)
                return false;

            var index = (int)key.AsNumber;

            return index >= 0 && index < _count;
        }

        public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator()
        {
            for (var i = 0; i < _count; ++i)
                yield return new KeyValuePair<Value, Value>(i, _generator(i));
        }

        public override bool TryGet(Value key, out Value value)
        {
            if (key.Type != ValueContent.Number)
            {
                value = default;

                return false;
            }

            var index = (int)key.AsNumber;

            if (index < 0 || index >= _count)
            {
                value = default;

                return false;
            }

            value = _generator(index);

            return true;
        }
    }
}