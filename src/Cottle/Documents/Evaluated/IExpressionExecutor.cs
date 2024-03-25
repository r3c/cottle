using System.IO;

namespace Cottle.Documents.Evaluated
{
    internal interface IExpressionExecutor
    {
        Value Execute(Runtime runtime, Frame frame, TextWriter output);
    }
}