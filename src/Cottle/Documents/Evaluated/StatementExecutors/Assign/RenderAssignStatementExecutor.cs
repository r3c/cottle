using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors.Assign
{
    internal class RenderAssignStatementExecutor : AssignStatementExecutor
    {
        private readonly IStatementExecutor _body;

        public RenderAssignStatementExecutor(Symbol symbol, IStatementExecutor body) :
            base(symbol)
        {
            _body = body;
        }

        protected override Value EvaluateOperand(Frame frame, TextWriter output)
        {
            using (var buffer = new StringWriter())
            {
                _body.Execute(frame, buffer);

                return buffer.ToString();
            }
        }
    }
}