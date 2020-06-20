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

        protected override DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration)
        {
            return Document.CreateDefault(template, configuration);
        }
    }
}