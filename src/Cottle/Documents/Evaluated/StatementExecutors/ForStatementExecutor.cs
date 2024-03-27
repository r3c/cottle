using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class ForStatementExecutor : IStatementExecutor
    {
        private readonly IStatementExecutor _body;

        private readonly IStatementExecutor? _empty;

        private readonly IExpressionExecutor _from;

        private readonly Symbol? _key;

        private readonly Symbol _value;

        public ForStatementExecutor(IExpressionExecutor from, Symbol? key, Symbol value, IStatementExecutor body, IStatementExecutor? empty)
        {
            _body = body;
            _empty = empty;
            _from = from;
            _key = key;
            _value = value;
        }

        public Value? Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            var fields = _from.Execute(runtime, frame, output).Fields;

            if (fields.Count > 0)
            {
                foreach (var pair in fields)
                {
                    runtime.Tick();

                    if (_key.HasValue)
                        frame.Locals[_key.Value.Index] = pair.Key;

                    frame.Locals[_value.Index] = pair.Value;

                    var result = _body.Execute(runtime, frame, output);

                    if (result.HasValue)
                        return result.Value;
                }
            }
            else if (_empty is not null)
            {
                var result = _empty.Execute(runtime, frame, output);

                if (result.HasValue)
                    return result.Value;
            }

            return null;
        }
    }
}