using System.IO;

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

        public Value? Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            while (_condition.Execute(runtime, frame, output).AsBoolean)
            {
                runtime.Tick();

                var result = _body.Execute(runtime, frame, output);

                if (result.HasValue)
                    return result.Value;
            }

            return null;
        }
    }
}