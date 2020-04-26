using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Executors
{
    internal class WrapExecutor : IExecutor
    {
        private readonly IExecutor _body;        
        private readonly IEvaluator _modifier;

        public WrapExecutor(IEvaluator modifier, IExecutor body)
        {
            _body = body;
            _modifier = modifier;
        }

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            var modifier = _modifier.Evaluate(frame, output);

            frame.Wrap(modifier.AsFunction);

            var stop = _body.Execute(frame, output, out result);

            frame.Unwrap();

            return stop;
        }
    }
}