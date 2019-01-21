using System.IO;

namespace Cottle.Documents.Simple
{
    internal interface IEvaluator
    {
        #region Methods

        Value Evaluate(IStore store, TextWriter output);

        string ToString();

        #endregion
    }
}