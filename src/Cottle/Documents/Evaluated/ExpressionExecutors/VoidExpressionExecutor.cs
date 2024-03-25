using System.IO;

namespace Cottle.Documents.Evaluated.ExpressionExecutors
{
    internal class VoidExpressionExecutor : IExpressionExecutor
    {
        public static readonly VoidExpressionExecutor Instance = new VoidExpressionExecutor();

        public Value Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            return Value.Undefined;
        }
    }
}