using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class WrapStatementExecutor : IStatementExecutor
    {
        private readonly IStatementExecutor _body;
        private readonly IExpressionExecutor _modifier;

        public WrapStatementExecutor(IExpressionExecutor modifier, IStatementExecutor body)
        {
            _body = body;
            _modifier = modifier;
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            var modifier = _modifier.Execute(frame, output);

            frame.Wrap(modifier.AsFunction);

            var result = _body.Execute(frame, output);

            frame.Unwrap();

            return result;
        }
    }
}