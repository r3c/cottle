using System.IO;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public class DefaultDocumentTester : IDocumentTester
    {
        [Test]
        [TestCase("declare", "as", "")]
        [TestCase("set", "to", "17")]
        public void RenderCommandSetFunctionScope(string command, string suffix, string expected)
        {
            AssertRender("{" + command + " f(x) " + suffix + ":{return x}}" +
                         "{set g(x) to:{return f(x)}}" +
                         "{g(17)}", expected);
        }

        protected override DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration)
        {
            return Document.CreateDefault(template, configuration);
        }
    }
}