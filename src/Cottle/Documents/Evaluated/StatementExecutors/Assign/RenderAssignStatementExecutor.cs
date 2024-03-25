using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors.Assign
{
    internal class RenderAssignStatementExecutor : AssignStatementExecutor
    {
        private readonly IStatementExecutor _body;

        public RenderAssignStatementExecutor(Symbol symbol, StoreMode mode, IStatementExecutor body) :
            base(symbol, mode)
        {
            _body = body;
        }

        protected override Value EvaluateOperand(Runtime runtime, Frame frame, TextWriter output)
        {
            using var buffer = new StringWriter();

            _body.Execute(runtime, frame, buffer);

            return buffer.ToString();
        }
    }
}