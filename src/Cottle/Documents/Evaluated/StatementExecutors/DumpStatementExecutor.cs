using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class DumpStatementExecutor : IStatementExecutor
    {
        private readonly IExpressionExecutor _expression;

        public DumpStatementExecutor(IExpressionExecutor expression)
        {
            _expression = expression;
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            output.Write(_expression.Execute(frame, output));

            return null;
        }
    }
}