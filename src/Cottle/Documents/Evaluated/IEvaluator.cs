using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated
{
    internal interface IEvaluator
    {
        Value Evaluate(Frame frame, TextWriter output);
    }
}