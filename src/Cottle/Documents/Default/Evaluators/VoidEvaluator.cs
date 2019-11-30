using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Default.Evaluators
{
    internal class VoidEvaluator : IEvaluator
    {
        public static readonly VoidEvaluator Instance = new VoidEvaluator();

        public Value Evaluate(Stack stack, TextWriter output)
        {
            return VoidValue.Instance;
        }
    }
}