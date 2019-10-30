using System.Globalization;
using System.IO;
using Cottle.Documents.Simple;
using Cottle.Settings;
using Cottle.Stores;

namespace Cottle.Documents
{
    /// <summary>
    /// Simple document renders templates using an interpreter. If offers better garbage collection and easier debugging
    /// but average rendering performance.
    /// </summary>
    public sealed class SimpleDocument : AbstractDocument
    {
        private readonly INode _renderer;

        private readonly ISetting _setting;

        public SimpleDocument(TextReader reader, ISetting setting)
        {
            var parser = ParserFactory.BuildParser(setting);
            var root = parser.Parse(reader);

            _renderer = Compiler.Compile(root);
            _setting = setting;
        }

        public SimpleDocument(TextReader reader) :
            this(reader, DefaultSetting.Instance)
        {
        }

        public SimpleDocument(string template, ISetting setting) :
            this(new StringReader(template), setting)
        {
        }

        public SimpleDocument(string template) :
            this(new StringReader(template), DefaultSetting.Instance)
        {
        }

        public override Value Render(IContext context, TextWriter writer)
        {
            _renderer.Render(new ContextStore(context), writer, out var result);

            return result;
        }

        public void Source(TextWriter writer)
        {
            _renderer.Source(_setting, writer);
        }

        public string Source()
        {
            var writer = new StringWriter(CultureInfo.InvariantCulture);

            Source(writer);

            return writer.ToString();
        }
    }
}