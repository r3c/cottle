using System.IO;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    [TestFixtureSource(typeof(DocumentConfigurationSource), nameof(DocumentConfigurationSource.Configurations))]
    public class EvaluatedDocumentTester : CompiledDocumentTester
    {
        public EvaluatedDocumentTester(DocumentConfiguration configuration) :
            base(configuration)
        {
        }

        [Test]
        public void Render_StatementDefine()
        {
            var result = AssertOutput("{define var}", string.Empty);

            Assert.That(result.Reports,
                Has.One.Matches<DocumentReport>(r =>
                    r.Severity == DocumentSeverity.Notice && r.Message.Contains("define")));
        }

        protected override DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration)
        {
            return Document.CreateDefault(template, configuration);
        }
    }
}