using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Cottle.Documents.Dynamic;
using Cottle.Stores;

namespace Cottle.Documents
{
    internal class NativeDocument : IDocument
    {
        private readonly DynamicFunction _root;

        public NativeDocument(Command root)
        {
            _root = new DynamicFunction(Enumerable.Empty<string>(), root);
        }

        public Value Render(IContext context, TextWriter writer)
        {
            return _root.Invoke(new ContextStore(context), Array.Empty<Value>(), writer);
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