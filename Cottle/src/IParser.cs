using System.IO;

namespace Cottle
{
    internal interface IParser
    {
        #region Methods

        Command Parse(TextReader reader);

        #endregion
    }
}