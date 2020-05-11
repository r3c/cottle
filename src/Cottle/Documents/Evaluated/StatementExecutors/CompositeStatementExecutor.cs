using System.Collections.Generic;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class CompositeStatementExecutor : IStatementExecutor
    {
        private readonly IReadOnlyList<IStatementExecutor> _nodes;

        public CompositeStatementExecutor(IReadOnlyList<IStatementExecutor> nodes)
        {
            _nodes = nodes;
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            foreach (var node in _nodes)
            {
                var result = node.Execute(frame, output);

                if (result.HasValue)
                    return result.Value;
            }

            return null;
        }
    }
}