using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Evaluators
{
    internal class VoidEvaluator : IEvaluator
    {
        public static readonly VoidEvaluator Instance = new VoidEvaluator();

        public Value Evaluate(Frame frame, TextWriter output)
        {
            return Value.Undefined;
        }
    }
}