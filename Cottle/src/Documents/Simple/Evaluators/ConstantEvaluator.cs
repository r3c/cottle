using System.IO;

namespace Cottle.Documents.Simple.Evaluators
{
    internal class ConstantEvaluator : IEvaluator
    {
        #region Attributes

        private readonly Value _value;

        #endregion

        #region Constructors

        public ConstantEvaluator(Value value)
        {
            _value = value;
        }

        #endregion

        #region Methods

        public Value Evaluate(IStore store, TextWriter output)
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        #endregion
    }
}