using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.ExpressionExecutors
{
    internal class AccessExpressionExecutor : IExpressionExecutor
    {
        public AccessExpressionExecutor(IExpressionExecutor source, IExpressionExecutor subscript)
        {
            _source = source;
            _subscript = subscript;
        }

        private readonly IExpressionExecutor _source;

        private readonly IExpressionExecutor _subscript;

        public Value Execute(Frame frame, TextWriter output)
        {
            var source = _source.Execute(frame, output);
            var subscript = _subscript.Execute(frame, output);

            return source.Fields[subscript];
        }
    }
}