using System;
using System.Collections;
using System.Collections.Generic;
using Cottle.Values;

namespace Cottle.Maps
{
    internal class GeneratorMap : AbstractMap
    {
        #region Constructors

        public GeneratorMap(Func<int, Value> generator, int count)
        {
            _count = count;
            _generator = generator;
        }

        #endregion

        #region Properties

        public override int Count => _count;

        #endregion

        #region Types

        private class GeneratorEnumerator : IEnumerator<KeyValuePair<Value, Value>>
        {
            #region Constructors

            public GeneratorEnumerator(Func<int, Value> generator, int count)
            {
                _count = count;
                Current = new KeyValuePair<Value, Value>(VoidValue.Instance, VoidValue.Instance);
                _generator = generator;
                _index = 0;
            }

            #endregion

            public KeyValuePair<Value, Value> Current { get; private set; }

            object IEnumerator.Current => Current;

            #region Attributes

            private readonly int _count;

            private readonly Func<int, Value> _generator;

            private int _index;

            #endregion

            #region Methods

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_index >= _count)
                    return false;

                Current = new KeyValuePair<Value, Value>(_index, _generator(_index));

                ++_index;

                return true;
            }

            public void Reset()
            {
                _index = 0;
            }

            #endregion
        }

        #endregion

        #region Attributes

        private readonly int _count;

        private readonly Func<int, Value> _generator;

        #endregion

        #region Methods

        public override bool Contains(Value key)
        {
            if (key.Type != ValueContent.Number)
                return false;

            var index = (int) key.AsNumber;

            return index >= 0 && index < _count;
        }

        public override IEnumerator<KeyValuePair<Value, Value>> GetEnumerator()
        {
            return new GeneratorEnumerator(_generator, _count);
        }

        public override bool TryGet(Value key, out Value value)
        {
            if (key.Type != ValueContent.Number)
            {
                value = default(Value);

                return false;
            }

            var index = (int) key.AsNumber;

            if (index < 0 || index >= _count)
            {
                value = default(Value);

                return false;
            }

            value = _generator(index);

            return true;
        }

        #endregion
    }
}