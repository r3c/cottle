using System.IO;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal class UnwrapStatementExecutor : IStatementExecutor
    {
        private readonly IStatementExecutor _body;

        public UnwrapStatementExecutor(IStatementExecutor body)
        {
            _body = body;
        }

        public Value? Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            var modifier = runtime.Unwrap();
            var result = _body.Execute(runtime, frame, output);

            runtime.Wrap(modifier);

            return result;
        }
    }
}