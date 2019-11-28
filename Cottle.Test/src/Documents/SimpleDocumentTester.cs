using System;
using System.IO;
using Cottle.Documents;
using Cottle.Settings;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public class SimpleDocumentTester : IDocumentTester
    {
        [Test]
        [TestCase("Hello, World!")]
        [TestCase("{0}")]
        [TestCase("{17}")]
        [TestCase("{declare a as 1}")]
        [TestCase("{dump 'string'}", "{dump \"string\"}")]
        [TestCase("{if 1:test}")]
        [TestCase("{for k, v in data:{k}={v}}")]
        [TestCase("{return [1, 2]}", "{return [0: 1, 1: 2]}")]
        [TestCase("{set b to 2}")]
        [TestCase("{while c:something}")]
        public void Source(string template, string expected = null)
        {
            var document = new SimpleDocument(template, new CustomSetting { Optimize = false });

            Assert.That(document.Source(), Is.EqualTo(expected ?? template));
        }

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

            return DocumentResult.CreateSuccess(new SimpleDocument(template, settings));
        }
    }
}