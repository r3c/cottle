using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Executors
{
    internal class ForExecutor : IExecutor
    {
        private readonly IExecutor _body;

        private readonly IExecutor _empty;

        private readonly IEvaluator _from;

        private readonly Action<Frame, Value> _key;

        private readonly Action<Frame, Value> _value;

        public ForExecutor(IEvaluator from, Symbol? key, Symbol value, IExecutor body, IExecutor empty)
        {
            _body = body;
            _empty = empty;
            _from = from;
            _key = key.HasValue ? Frame.CreateSetter(key.Value) : null;
            _value = Frame.CreateSetter(value);
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            var fields = _from.Evaluate(frame, output).Fields;

            if (fields.Count > 0)
            {
                foreach (var pair in fields)
                {
                    if (_key != null)
                        _key(frame, pair.Key);

                    _value(frame, pair.Value);

                    if (_body.Execute(frame, output, out result))
                        return true;
                }
            }
            else if (_empty != null && _empty.Execute(frame, output, out result))
                return true;

            result = Value.Undefined;

            return false;
        }
    }
}