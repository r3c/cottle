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

        protected override Value Evaluate(Stack stack, TextWriter output)
        {
            using (var buffer = new StringWriter())
            {
                _body.Execute(stack, buffer, out _);

                return buffer.ToString();
            }
        }
    }
}