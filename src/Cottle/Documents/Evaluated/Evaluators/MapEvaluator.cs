using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;
using Cottle.Values;

namespace Cottle.Documents.Evaluated.Evaluators
{
    internal class MapEvaluator : IEvaluator
    {
        private readonly IReadOnlyList<KeyValuePair<IEvaluator, IEvaluator>> _elements;

        public MapEvaluator(IReadOnlyList<KeyValuePair<IEvaluator, IEvaluator>> elements)
        {
            _elements = elements;
        }

        public Value Evaluate(Frame frame, TextWriter writer)
        {
            return new MapValue(_elements.Select(element =>
            {
                var key = element.Key.Evaluate(frame, writer);
                var value = element.Value.Evaluate(frame, writer);

                return new KeyValuePair<Value, Value>(key, value);
            }));
        }
    }
}