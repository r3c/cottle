using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Evaluators
{
    internal class VoidEvaluator : IEvaluator
    {
        public static readonly VoidEvaluator Instance = new VoidEvaluator();

        public Value Evaluate(IStore store, TextWriter output)
        {
            return VoidValue.Instance;
        }

        public override string ToString()
        {
            return VoidValue.Instance.ToString();
        }
    }
}