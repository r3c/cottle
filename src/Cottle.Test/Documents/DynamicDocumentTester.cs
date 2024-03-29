using System;
using System.IO;
using Cottle.Documents;
using Cottle.Settings;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    [TestFixtureSource(typeof(DocumentConfigurationSource), nameof(DocumentConfigurationSource.Configurations))]
    public class DynamicDocumentTester : DocumentTester
    {
        public DynamicDocumentTester(DocumentConfiguration configuration) :
            base(configuration)
        {
        }

        protected override DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration)
        {
            var trimmer = configuration.Trimmer ?? DocumentConfiguration.DefaultTrimmer;

            var settings = new CustomSetting
            {
                BlockBegin = configuration.BlockBegin ?? DefaultSetting.Instance.BlockBegin,
                BlockContinue = configuration.BlockContinue ?? DefaultSetting.Instance.BlockContinue,
                BlockEnd = configuration.BlockEnd ?? DefaultSetting.Instance.BlockEnd,
                Escape = configuration.Escape.GetValueOrDefault('\\'),
                Optimize = !configuration.NoOptimize,
                Trimmer = s => trimmer(s)
            };

#pragma warning disable 618
            return DocumentResult.CreateSuccess(new DynamicDocument(template, settings), Array.Empty<DocumentReport>());
#pragma warning restore 618
        }
    }
}