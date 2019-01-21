using System.Globalization;
using System.IO;

namespace Cottle.Documents
{
    public abstract class AbstractDocument : IDocument
    {
        #region Methods / Abstract

        public abstract Value Render(IStore store, TextWriter writer);

        #endregion

        #region Methods / Public

        public string Render(IStore store)
        {
            var writer = new StringWriter(CultureInfo.InvariantCulture);

            Render(store, writer);

            return writer.ToString();
        }

        #endregion
    }
}