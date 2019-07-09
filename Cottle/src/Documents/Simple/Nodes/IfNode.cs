using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
    internal class IfNode : INode
    {
        public IfNode(IEnumerable<KeyValuePair<IEvaluator, INode>> branches, INode fallback)
        {
            _branches = branches.ToArray();
            _fallback = fallback;
        }

        private readonly KeyValuePair<IEvaluator, INode>[] _branches;

        private readonly INode _fallback;

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            bool halt;

            foreach (var branch in _branches)
                if (branch.Key.Evaluate(store, output).AsBoolean)
                {
                    store.Enter();

                    halt = branch.Value.Render(store, output, out result);

                    store.Leave();

                    return halt;
                }

            if (_fallback != null)
            {
                store.Enter();

                halt = _fallback.Render(store, output, out result);

                store.Leave();

                return halt;
            }

            result = VoidValue.Instance;

            return false;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            var first = true;

            foreach (var branch in _branches)
            {
                if (first)
                {
                    output.Write(setting.BlockBegin);
                    output.Write("if ");

                    first = false;
                }
                else
                {
                    output.Write(setting.BlockContinue);
                    output.Write("elif ");
                }

                output.Write(branch.Key);
                output.Write(":");

                branch.Value.Source(setting, output);
            }

            if (_fallback != null)
            {
                output.Write(setting.BlockContinue);
                output.Write("else:");

                _fallback.Source(setting, output);
            }

            output.Write(setting.BlockEnd);
        }
    }
}