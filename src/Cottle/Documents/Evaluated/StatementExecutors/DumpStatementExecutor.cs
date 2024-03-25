using System.IO;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class DumpStatementExecutor : IStatementExecutor
    {
        private readonly IExpressionExecutor _expression;

        public DumpStatementExecutor(IExpressionExecutor expression)
        {
            _expression = expression;
        }

        public Value? Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            output.Write(_expression.Execute(runtime, frame, output));

            return null;
        }
    }
}