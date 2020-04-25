using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cottle.Documents.Simple.Evaluators
{
    internal class MapEvaluator : IEvaluator
    {
        private readonly KeyValuePair<IEvaluator, IEvaluator>[] _elements;

        public MapEvaluator(IEnumerable<KeyValuePair<IEvaluator, IEvaluator>> elements)
        {
            _elements = elements.ToArray();
        }

        public Value Evaluate(IStore store, TextWriter writer)
        {
            return Value.FromEnumerable(_elements.Select(element =>
            {
                var key = element.Key.Evaluate(store, writer);
                var value = element.Value.Evaluate(store, writer);

                return new KeyValuePair<Value, Value>(key, value);
            }));
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var comma = false;

            builder.Append('[');

            foreach (var element in _elements)
            {
                if (comma)
                    builder.Append(", ");
                else
                    comma = true;

                builder.Append(element.Key);
                builder.Append(": ");
                builder.Append(element.Value);
            }

            builder.Append(']');

            return builder.ToString();
        }
    }
}