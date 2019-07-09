using System.IO;
using System.Text;
using Cottle.Values;

namespace Cottle.Documents.Simple.Evaluators
{
    internal class AccessEvaluator : IEvaluator
    {
        public AccessEvaluator(IEvaluator source, IEvaluator subscript)
        {
            _source = source;
            _subscript = subscript;
        }

        private readonly IEvaluator _source;

        private readonly IEvaluator _subscript;

        public Value Evaluate(IStore store, TextWriter output)
        {
            var key = _subscript.Evaluate(store, output);
            var map = _source.Evaluate(store, output);

            if (map.Fields.TryGet(key, out var value))
                return value;

            return VoidValue.Instance;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append(_source)
                .Append('[')
                .Append(_subscript)
                .Append(']')
                .ToString();
        }
    }
}