using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
    internal class DumpNode : INode
    {
        private readonly IEvaluator _expression;

        public DumpNode(IEvaluator expression)
        {
            _expression = expression;
        }

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            output.Write(_expression.Evaluate(store, output));

            result = VoidValue.Instance;

            return false;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            output.Write(setting.BlockBegin);
            output.Write("dump ");
            output.Write(_expression);
            output.Write(setting.BlockEnd);
        }
    }
}