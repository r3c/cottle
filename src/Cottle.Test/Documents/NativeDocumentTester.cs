using System.IO;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public class NativeDocumentTester : IDocumentTester
    {
        protected override DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration)
        {
            return Document.CreateNative(template, configuration);
        }
    }
}