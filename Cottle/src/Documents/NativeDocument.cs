using System.Globalization;
using System.IO;
using System.Linq;
using Cottle.Documents.Dynamic;
using Cottle.Stores;

namespace Cottle.Documents
{
    internal class NativeDocument : IDocument
    {
        private readonly Function _root;

        public NativeDocument(Command root)
        {
            _root = new Function(Enumerable.Empty<string>(), root);
        }

        public Value Render(IContext context, TextWriter writer)
        {
            return _root.Execute(null, new ContextStore(context), writer);
        }

        public string Render(IContext context)
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                Render(context, writer);

                return writer.ToString();
            }
        }
    }
}