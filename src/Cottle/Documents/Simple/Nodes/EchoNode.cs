using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
    internal class EchoNode : INode
    {
        private readonly IEvaluator _expression;

        public EchoNode(IEvaluator expression)
        {
            _expression = expression;
        }

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            output.Write(_expression.Evaluate(store, output).AsString);

            result = VoidValue.Instance;

            return false;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            var source = _expression.ToString();

            output.Write(setting.BlockBegin);

            if (source.StartsWith("echo", StringComparison.Ordinal))
                output.Write("echo ");

            output.Write(source);
            output.Write(setting.BlockEnd);
        }
    }
}