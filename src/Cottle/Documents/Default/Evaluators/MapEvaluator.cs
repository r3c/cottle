using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Default.Evaluators
{
    internal class MapEvaluator : IEvaluator
    {
        private readonly KeyValuePair<IEvaluator, IEvaluator>[] _elements;

        public MapEvaluator(IEnumerable<KeyValuePair<IEvaluator, IEvaluator>> elements)
        {
            _elements = elements.ToArray();
        }

        public Value Evaluate(Stack stack, TextWriter writer)
        {
            return new MapValue(_elements.Select(element =>
            {
                var key = element.Key.Evaluate(stack, writer);
                var value = element.Value.Evaluate(stack, writer);

                return new KeyValuePair<Value, Value>(key, value);
            }));
        }
    }
}