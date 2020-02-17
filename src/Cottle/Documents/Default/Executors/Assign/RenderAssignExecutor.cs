using System.IO;

namespace Cottle.Documents.Default.Executors.Assign
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