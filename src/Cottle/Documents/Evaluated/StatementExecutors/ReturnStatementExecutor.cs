using System.IO;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class ReturnStatementExecutor : IStatementExecutor
    {
        private readonly IExpressionExecutor _expression;

        public ReturnStatementExecutor(IExpressionExecutor expression)
        {
            _expression = expression;
        }

        public Value? Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            return _expression.Execute(runtime, frame, output);
        }
    }
}