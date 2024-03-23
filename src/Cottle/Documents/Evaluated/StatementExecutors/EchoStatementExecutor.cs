using System;
using System.IO;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class EchoStatementExecutor : IStatementExecutor
    {
        private readonly IExpressionExecutor _expression;

        public EchoStatementExecutor(IExpressionExecutor expression)
        {
            _expression = expression;
        }

        public Value? Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            var subject = _expression.Execute(runtime, frame, output);
            var value = frame.Echo(Tuple.Create(runtime, frame), subject, output);

            output.Write(value);

            return null;
        }
    }
}