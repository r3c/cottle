using System.IO;

namespace Cottle.Documents.Simple
{
    internal interface IEvaluator
    {
        Value Evaluate(IStore store, TextWriter output);

        string ToString();
    }
}