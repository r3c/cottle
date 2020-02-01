using System.IO;
using System.Linq;
using Cottle.Parsers;
using NUnit.Framework;

namespace Cottle.Test.Parsers
{
    public class ForwardParserTester
    {
        [Test]
        [TestCase("{for i in c:x}", "", "i", ExpressionType.Symbol, CommandType.Literal, CommandType.None)]
        [TestCase("{for k, v in []:{echo i}|empty:x}", "k", "v", ExpressionType.Map, CommandType.Echo, CommandType.Literal)]
        public void Parse_CommandFor(string template, string expectedKey, string expectedValue, int expectedOperand,
            int expectedBody, int expectedNext)
        {
            var command = ForwardParserTester.CreateAndParse(template);

            Assert.That(command.Body.Type, Is.EqualTo((CommandType)expectedBody));
            Assert.That(command.Key, Is.EqualTo(expectedKey));
            Assert.That(command.Value, Is.EqualTo(expectedValue));
            Assert.That(command.Operand.Type, Is.EqualTo((ExpressionType)expectedOperand));
            Assert.That(command.Next.Type, Is.EqualTo((CommandType)expectedNext));
            Assert.That(command.Type, Is.EqualTo(CommandType.For));
        }

        [TestCase("{1", 2, 0, "expected end of block, found end of stream")]
        [TestCase("{1.2.3", 5, 1, "expected field name, found 3")]
        [TestCase("{\"abc", 1, 4, "expected expression, found unfinished string")]
        [TestCase("a{1", 3, 0, "expected end of block, found end of stream")]
        [TestCase("a{1+}", 4, 1, "expected expression, found unexpected character")]
        public void Parse_Reports(string template, int expectedOffset, int expectedLength, string expectedMessage)
        {
            var parser = ForwardParserTester.Create();

            using (var reader = new StringReader(template))
            {
                Assert.That(parser.Parse(reader, out _, out var reports), Is.False);

                var firstError = reports.FirstOrDefault(r => r.Severity == DocumentSeverity.Error);

                Assert.That(firstError.Length, Is.EqualTo(expectedLength));
                Assert.That(firstError.Message, Does.Contain(expectedMessage));
                Assert.That(firstError.Offset, Is.EqualTo(expectedOffset));
            }
        }

        private static ForwardParser Create()
        {
            return new ForwardParser("{", "|", "}", '\\', s => s);
        }

        private static Command CreateAndParse(string template)
        {
            var parser = ForwardParserTester.Create();

            using (var reader = new StringReader(template))
            {
                Assert.That(parser.Parse(reader, out var command, out _), Is.True);

                return command;
            }
        }
    }
}