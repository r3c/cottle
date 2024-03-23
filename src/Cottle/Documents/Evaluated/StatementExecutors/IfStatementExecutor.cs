using System.Collections.Generic;
using System.IO;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class IfStatementExecutor : IStatementExecutor
    {
        private readonly IReadOnlyList<KeyValuePair<IExpressionExecutor, IStatementExecutor>> _branches;

        private readonly IStatementExecutor? _fallback;

        public IfStatementExecutor(IReadOnlyList<KeyValuePair<IExpressionExecutor, IStatementExecutor>> branches,
            IStatementExecutor? fallback)
        {
            _branches = branches;
            _fallback = fallback;
        }

        public Value? Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            foreach (var branch in _branches)
            {
                if (!branch.Key.Execute(runtime, frame, output).AsBoolean)
                    continue;

                return branch.Value.Execute(runtime, frame, output);
            }

            if (_fallback is not null)
                return _fallback.Execute(runtime, frame, output);

            return null;
        }
    }
}