using System.IO;

namespace Cottle.Documents.Default
{
    internal interface IEvaluator
    {
        Value Evaluate(Stack stack, TextWriter output);
    }
}