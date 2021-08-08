using System.IO;

namespace Cottle.Documents.Simple.Evaluators
{
    internal class VoidEvaluator : IEvaluator
    {
        public static readonly VoidEvaluator Instance = new VoidEvaluator();

        public Value Evaluate(IStore store, TextWriter output)
        {
            return Value.Undefined;
        }

        public override string? ToString()
        {
            return Value.Undefined.ToString();
        }
    }
}