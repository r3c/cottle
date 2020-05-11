using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.ExpressionExecutors
{
    internal class VoidExpressionExecutor : IExpressionExecutor
    {
        public static readonly VoidExpressionExecutor Instance = new VoidExpressionExecutor();

        public Value Execute(Frame frame, TextWriter output)
        {
            return Value.Undefined;
        }
    }
}