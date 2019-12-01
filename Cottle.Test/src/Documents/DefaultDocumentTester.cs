using System.IO;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public class DefaultDocumentTester : IDocumentTester
    {
        protected override DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration)
        {
            return Document.CreateDefault(template, configuration);
        }
    }
}