using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class UnwrapStatementExecutor : IStatementExecutor
    {
        private readonly IStatementExecutor _body;

        public UnwrapStatementExecutor(IStatementExecutor body)
        {
            _body = body;
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            var modifier = frame.Unwrap();
            var result = _body.Execute(frame, output);

            frame.Wrap(modifier);

            return result;
        }
    }
}