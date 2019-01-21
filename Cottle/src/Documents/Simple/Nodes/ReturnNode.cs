using System.IO;

namespace Cottle.Documents.Simple.Nodes
{
    internal class ReturnNode : INode
    {
        #region Attributes

        private readonly IEvaluator _expression;

        #endregion

        #region Constructors

        public ReturnNode(IEvaluator expression)
        {
            _expression = expression;
        }

        #endregion

        #region Methods

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            result = _expression.Evaluate(store, output);

            return true;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            output.Write(setting.BlockBegin);
            output.Write("return ");
            output.Write(_expression);
            output.Write(setting.BlockEnd);
        }

        #endregion
    }
}