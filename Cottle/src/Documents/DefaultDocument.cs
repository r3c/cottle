using System.Globalization;
using System.IO;
using Cottle.Documents.Simple;
using Cottle.Stores;

namespace Cottle.Documents
{
    internal class DefaultDocument : IDocument
    {
        private readonly INode _root;

        public DefaultDocument(Command root)
        {
            _root = Compiler.Compile(root);
        }

        public Value Render(IContext context, TextWriter writer)
        {
            _root.Render(new ContextStore(context), writer, out var result);

            return result;
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