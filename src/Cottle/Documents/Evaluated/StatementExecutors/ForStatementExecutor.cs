using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class ForStatementExecutor : IStatementExecutor
    {
        private readonly IStatementExecutor _body;

        private readonly IStatementExecutor? _empty;

        private readonly IExpressionExecutor _from;

        private readonly Action<Frame, Value>? _key;

        private readonly Action<Frame, Value> _value;

        public ForStatementExecutor(IExpressionExecutor from, Symbol? key, Symbol value, IStatementExecutor body,
            IStatementExecutor? empty)
        {
            _body = body;
            _empty = empty;
            _from = from;
            _key = key.HasValue ? Frame.CreateSetter(key.Value) : null;
            _value = Frame.CreateSetter(value);
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            var fields = _from.Execute(frame, output).Fields;

            if (fields.Count > 0)
            {
                foreach (var pair in fields)
                {
                    if (_key != null)
                        _key(frame, pair.Key);

                    _value(frame, pair.Value);

                    var result = _body.Execute(frame, output);

                    if (result.HasValue)
                        return result.Value;
                }
            }
            else if (_empty != null)
            {
                var result = _empty.Execute(frame, output);

                if (result.HasValue)
                    return result.Value;
            }

            return null;
        }
    }
}