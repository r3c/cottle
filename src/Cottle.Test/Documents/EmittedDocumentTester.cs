using System.IO;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    [TestFixtureSource(typeof(DocumentConfigurationSource), nameof(DocumentConfigurationSource.Configurations))]
    public class EmittedDocumentTester : EvaluatedDocumentTester
    {
        public EmittedDocumentTester(DocumentConfiguration configuration) :
            base(configuration)
        {
        }

        protected override DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration)
        {
            return Document.CreateNative(template, configuration);
        }
    }
}