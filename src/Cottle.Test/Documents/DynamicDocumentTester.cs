using System.IO;
using Cottle.Documents;
using Cottle.Settings;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public class DynamicDocumentTester : IDocumentTester
    {
        protected override DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration)
        {
            var trimmer = configuration.Trimmer ?? (s => s);

            var settings = new CustomSetting
            {
                BlockBegin = configuration.BlockBegin,
                BlockContinue = configuration.BlockContinue,
                BlockEnd = configuration.BlockEnd,
                Escape = configuration.Escape.GetValueOrDefault('\\'),
                Optimize = !configuration.NoOptimize,
                Trimmer = s => trimmer(s)
            };

#pragma warning disable 618
            return DocumentResult.CreateSuccess(new DynamicDocument(template, settings));
#pragma warning restore 618
        }
    }
}