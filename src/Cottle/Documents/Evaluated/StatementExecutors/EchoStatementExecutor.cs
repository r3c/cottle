using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class EchoStatementExecutor : IStatementExecutor
    {
        private readonly IExpressionExecutor _expression;

        public EchoStatementExecutor(IExpressionExecutor expression)
        {
            _expression = expression;
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            var subject = _expression.Execute(frame, output);
            var value = frame.Echo(subject, output);

            output.Write(value);

            return null;
        }
    }
}