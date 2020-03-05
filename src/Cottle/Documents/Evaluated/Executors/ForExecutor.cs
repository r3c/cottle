using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Values;

namespace Cottle.Documents.Evaluated.Executors
{
    internal class ForExecutor : IExecutor
    {
        private readonly IExecutor _body;

        private readonly IExecutor _empty;

        private readonly IEvaluator _from;

        private readonly int? _key;

        private readonly int _value;

        public ForExecutor(IEvaluator from, int? key, int value, IExecutor body, IExecutor empty)
        {
            _body = body;
            _empty = empty;
            _from = from;
            _key = key;
            _value = value;
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            var fields = _from.Evaluate(frame, output).Fields;

            if (fields.Count > 0)
            {
                foreach (var pair in fields)
                {
                    if (_key.HasValue)
                        frame.Locals[_key.Value] = pair.Key;

                    frame.Locals[_value] = pair.Value;

                    if (_body.Execute(frame, output, out result))
                        return true;
                }
            }
            else if (_empty != null && _empty.Execute(frame, output, out result))
                return true;

            result = VoidValue.Instance;

            return false;
        }
    }
}