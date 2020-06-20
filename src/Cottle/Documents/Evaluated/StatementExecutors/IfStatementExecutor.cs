using System.Collections.Generic;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class IfStatementExecutor : IStatementExecutor
    {
        private readonly IReadOnlyList<KeyValuePair<IExpressionExecutor, IStatementExecutor>> _branches;

        private readonly IStatementExecutor _fallback;

        public IfStatementExecutor(IReadOnlyList<KeyValuePair<IExpressionExecutor, IStatementExecutor>> branches, IStatementExecutor fallback)
        {
            _branches = branches;
            _fallback = fallback;
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            foreach (var branch in _branches)
            {
                if (!branch.Key.Execute(frame, output).AsBoolean)
                    continue;

                return branch.Value.Execute(frame, output);
            }

            if (_fallback != null)
                return _fallback.Execute(frame, output);

            return null;
        }
    }
}