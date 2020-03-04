using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;

namespace Cottle.Documents.Evaluated.Executors.Assign
{
    internal class RenderAssignExecutor : AssignExecutor
    {
        private readonly IExecutor _body;

        public RenderAssignExecutor(Symbol symbol, IExecutor body) :
            base(symbol)
        {
            _body = body;
        }

        protected override Value Evaluate(Frame frame, TextWriter output)
        {
            using (var buffer = new StringWriter())
            {
                _body.Execute(frame, buffer, out _);

                return buffer.ToString();
            }
        }
    }
}