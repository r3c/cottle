using System.IO;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public class EmittedDocumentTester : EvaluatedDocumentTester
    {
        protected override DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration)
        {
            return Document.CreateNative(template, configuration);
        }
    }
}