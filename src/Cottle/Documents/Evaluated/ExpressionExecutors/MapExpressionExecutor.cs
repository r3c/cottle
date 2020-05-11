using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.ExpressionExecutors
{
    internal class MapExpressionExecutor : IExpressionExecutor
    {
        private readonly IReadOnlyList<KeyValuePair<IExpressionExecutor, IExpressionExecutor>> _elements;

        public MapExpressionExecutor(IReadOnlyList<KeyValuePair<IExpressionExecutor, IExpressionExecutor>> elements)
        {
            _elements = elements;
        }

        public Value Execute(Frame frame, TextWriter writer)
        {
            return Value.FromEnumerable(_elements.Select(element =>
            {
                var key = element.Key.Execute(frame, writer);
                var value = element.Value.Execute(frame, writer);

                return new KeyValuePair<Value, Value>(key, value);
            }));
        }
    }
}