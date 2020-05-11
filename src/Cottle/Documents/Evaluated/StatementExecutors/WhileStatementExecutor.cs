using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class WhileStatementExecutor : IStatementExecutor
    {
        public WhileStatementExecutor(IExpressionExecutor condition, IStatementExecutor body)
        {
            _body = body;
            _condition = condition;
        }

        private readonly IStatementExecutor _body;

        private readonly IExpressionExecutor _condition;

        public Value? Execute(Frame frame, TextWriter output)
        {
            while (_condition.Execute(frame, output).AsBoolean)
            {
                var result = _body.Execute(frame, output);

                if (result.HasValue)
                    return result.Value;
            }

            return null;
        }
    }
}