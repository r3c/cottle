using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
    internal class WhileNode : INode
    {
        #region Constructors

        public WhileNode(IEvaluator condition, INode body)
        {
            _body = body;
            _condition = condition;
        }

        #endregion

        #region Attributes

        private readonly INode _body;

        private readonly IEvaluator _condition;

        #endregion

        #region Methods

        public bool Render(IStore store, TextWriter output, out Value result)
        {
            while (_condition.Evaluate(store, output).AsBoolean)
            {
                store.Enter();

                if (_body.Render(store, output, out result))
                {
                    store.Leave();

                    return true;
                }

                store.Leave();
            }

            result = VoidValue.Instance;

            return false;
        }

        public void Source(ISetting setting, TextWriter output)
        {
            output.Write(setting.BlockBegin);
            output.Write("while ");
            output.Write(_condition);
            output.Write(":");

            _body.Source(setting, output);

            output.Write(setting.BlockEnd);
        }

        #endregion
    }
}