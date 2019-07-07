using System.Globalization;
using System.IO;

namespace Cottle.Documents
{
    public abstract class AbstractDocument : IDocument
    {
        #region Methods / Abstract

        public abstract Value Render(IContext context, TextWriter writer);

        #endregion

        #region Methods / Public

        public string Render(IContext context)
        {
            var writer = new StringWriter(CultureInfo.InvariantCulture);

            Render(context, writer);

            return writer.ToString();
        }

        #endregion
    }
}