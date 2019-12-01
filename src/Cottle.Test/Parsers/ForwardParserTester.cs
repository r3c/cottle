using System.IO;
using System.Linq;
using Cottle.Parsers;
using NUnit.Framework;

namespace Cottle.Test.Parsers
{
    public class ForwardParserTester
    {
        [TestCase("{1", 2, 0, "expected end of block, found end of stream")]
        [TestCase("{1.2.3", 5, 1, "expected field name, found 3")]
        [TestCase("{\"abc", 1, 4, "expected expression, found unfinished string")]
        [TestCase("a{1", 3, 0, "expected end of block, found end of stream")]
        [TestCase("a{1+}", 4, 1, "expected expression, found unexpected character")]
        public void Parse_Reports(string template, int expectedOffset, int expectedLength, string expectedMessage)
        {
            var parser = new ForwardParser("{", "[", "}", '\\', s => s);

            using (var reader = new StringReader(template))
            {
                Assert.That(parser.Parse(reader, out _, out var reports), Is.False);

                var firstError = reports.FirstOrDefault(r => r.Severity == DocumentSeverity.Error);

                Assert.That(firstError.Length, Is.EqualTo(expectedLength));
                Assert.That(firstError.Message, Does.Contain(expectedMessage));
                Assert.That(firstError.Offset, Is.EqualTo(expectedOffset));
            }
        }
    }
}