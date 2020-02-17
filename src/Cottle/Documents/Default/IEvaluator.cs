using System.IO;

namespace Cottle.Documents.Default
{
    internal interface IEvaluator
    {
        Value Evaluate(Frame frame, TextWriter output);
    }
}