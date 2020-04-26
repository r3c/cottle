using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Executors
{
    internal class UnwrapExecutor : IExecutor
    {
        private readonly IExecutor _body;        

        public UnwrapExecutor(IExecutor body)
        {
            _body = body;
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            var modifier = frame.Unwrap();
            var stop = _body.Execute(frame, output, out result);

            frame.Wrap(modifier);

            return stop;
        }
    }
}