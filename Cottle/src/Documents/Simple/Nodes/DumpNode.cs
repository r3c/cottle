using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
    internal class DumpNode : INode
    {
        #region Attributes

        private readonly IEvaluator _expression;

        #endregion

        #region Constructors

        public DumpNode(IEvaluator expression)
        {
            _expression = expression;
        }

        #endregion

        #region Methods

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

        #endregion
    }
}