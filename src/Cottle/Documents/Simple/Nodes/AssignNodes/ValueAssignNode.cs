using System.IO;

namespace Cottle.Documents.Simple.Nodes.AssignNodes
{
    internal class ValueAssignNode : AssignNode
    {
        private readonly IEvaluator _expression;

        public ValueAssignNode(string name, IEvaluator expression, StoreMode mode) :
            base(name, mode)
        {
            _expression = expression;
        }

        protected override Value Evaluate(IStore store, TextWriter output)
        {
            return _expression.Evaluate(store, output);
        }

        protected override void SourceSymbol(string name, TextWriter output)
        {
            output.Write(name);
        }

        protected override void SourceValue(ISetting setting, TextWriter output)
        {
            output.Write(_expression);
        }
    }
}