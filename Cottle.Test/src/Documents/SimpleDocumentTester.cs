using Cottle.Documents;
using Cottle.Settings;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public class SimpleDocumentTester
    {
        [Test]
        [TestCase("Hello, World!")]
        [TestCase("{0}")]
        [TestCase("{17}")]
        [TestCase("{1 + 1}", "{<native()>(1, 1)}")]
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
    }
}