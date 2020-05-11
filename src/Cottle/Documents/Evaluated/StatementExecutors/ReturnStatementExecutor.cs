using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class ReturnStatementExecutor : IStatementExecutor
    {
        private readonly IExpressionExecutor _expression;

        public ReturnStatementExecutor(IExpressionExecutor expression)
        {
            _expression = expression;
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            return _expression.Execute(frame, output);
        }
    }
}