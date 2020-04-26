using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cottle.Parsers;
using NUnit.Framework;

namespace Cottle.Test.Parsers
{
    public class ForwardParserTester
    {
        [Test]
        [TestCase("<|", "|>", "1 || 0", "true")]
        [TestCase("<&", "&>", "1 && 0", "")]
        public void Parse_Ambiguous_Block(string blockBegin, string blockEnd, string expression, string expected)
        {
            var configuration = new DocumentConfiguration
                { BlockBegin = blockBegin, BlockContinue = "<>", BlockEnd = blockEnd };
            var statement = ForwardParserTester.Parse(ForwardParserTester.Create(configuration),
                blockBegin + expression + blockEnd);

            Assert.That(statement.Type, Is.EqualTo(StatementType.Echo));
            Assert.That(statement.Operand.Type, Is.EqualTo(ExpressionType.Invoke));
            Assert.That(statement.Operand.Arguments.Count, Is.EqualTo(2));
            Assert.That(statement.Operand.Arguments[0].Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(statement.Operand.Arguments[0].Value, Is.EqualTo((Value)1));
            Assert.That(statement.Operand.Arguments[1].Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(statement.Operand.Arguments[1].Value, Is.EqualTo((Value)0));
        }

        [Test]
        [TestCase("declare")]
        [TestCase("dump")]
        [TestCase("echo")]
        [TestCase("for")]
        [TestCase("if")]
        [TestCase("set")]
        [TestCase("unwrap")]
        [TestCase("while")]
        [TestCase("wrap")]
        public void Parse_Ambiguous_Echo(string name)
        {
            var statement = ForwardParserTester.Parse(ForwardParserTester.Create(), "{" + name + "}");

            Assert.That(statement.Type, Is.EqualTo(StatementType.Echo));
            Assert.That(statement.Operand.Type, Is.EqualTo(ExpressionType.Symbol));
            Assert.That(statement.Operand.Value, Is.EqualTo((Value)name));
        }

        [Test]
        [TestCase("{for i in c:x}", "", "i", ExpressionType.Symbol, StatementType.Literal, StatementType.None)]
        [TestCase("{for k, v in []:{echo i}|empty:x}", "k", "v", ExpressionType.Map, StatementType.Echo, StatementType.Literal)]
        public void Parse_StatementFor(string template, string expectedKey, string expectedValue, int expectedOperand,
            int expectedBody, int expectedNext)
        {
            var statement = ForwardParserTester.Parse(ForwardParserTester.Create(), template);

            Assert.That(statement.Body.Type, Is.EqualTo((StatementType)expectedBody));
            Assert.That(statement.Key, Is.EqualTo(expectedKey));
            Assert.That(statement.Value, Is.EqualTo(expectedValue));
            Assert.That(statement.Operand.Type, Is.EqualTo((ExpressionType)expectedOperand));
            Assert.That(statement.Next.Type, Is.EqualTo((StatementType)expectedNext));
            Assert.That(statement.Type, Is.EqualTo(StatementType.For));
        }

        [Test]
        [TestCase("{unwrap:test}", StatementType.Literal)]
        [TestCase("{unwrap:{echo 5}}", StatementType.Echo)]
        public void Parse_StatementUnwrap(string template, int expectedbody)
        {
            var statement = ForwardParserTester.Parse(ForwardParserTester.Create(), template);

            Assert.That(statement.Body.Type, Is.EqualTo((StatementType)expectedbody));
            Assert.That(statement.Type, Is.EqualTo(StatementType.Unwrap));
        }

        [Test]
        [TestCase("{wrap 42:test}", ExpressionType.Constant, StatementType.Literal)]
        [TestCase("{wrap f:{echo 5}}", ExpressionType.Symbol, StatementType.Echo)]
        public void Parse_StatementWrap(string template, int expectedOperand, int expectedbody)
        {
            var statement = ForwardParserTester.Parse(ForwardParserTester.Create(), template);

            Assert.That(statement.Body.Type, Is.EqualTo((StatementType)expectedbody));
            Assert.That(statement.Operand.Type, Is.EqualTo((ExpressionType)expectedOperand));
            Assert.That(statement.Type, Is.EqualTo(StatementType.Wrap));
        }

        [Test]
        [TestCase("{\"\\\"\"}", '\\', "\"")]
        [TestCase("{\"xxxy\"}", 'x', "xy")]
        public void Parse_ConfigurationEscapeStatement(string template, char escape, string expected)
        {
            var configuration = new DocumentConfiguration { Escape = escape };
            var statement = ForwardParserTester.Parse(ForwardParserTester.Create(configuration), template);

            Assert.That(statement.Type, Is.EqualTo(StatementType.Echo));
            Assert.That(statement.Operand.Type, Is.EqualTo(ExpressionType.Constant));
            Assert.That(statement.Operand.Value, Is.EqualTo((Value)expected));
        }

        [Test]
        [TestCase("Escaped\\ Literal", '\\', "Escaped Literal")]
        [TestCase("-{'x'-}", '-', "{'x'}")]
        [TestCase("ABC", 'x', "ABC")]
        [TestCase("--", '-', "-")]
        public void Parse_ConfigurationEscapeLiteral(string template, char escape, string expected)
        {
            var configuration = new DocumentConfiguration { Escape = escape };
            var statement = ForwardParserTester.Parse(ForwardParserTester.Create(configuration), template);

            Assert.That(statement.Type, Is.EqualTo(StatementType.Literal));
            Assert.That(statement.Value, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("A", "A", "B", "B")]
        [TestCase("X  Y", " +", " ", "X Y")]
        [TestCase("df98gd76dfg5df4g321gh0", "[^0-9]", "", "9876543210")]
        public void Parse_ConfigurationTrimmer(string template, string pattern, string replacement, string expected)
        {
            var configuration = new DocumentConfiguration { Trimmer = s => Regex.Replace(s, pattern, replacement) };
            var statement = ForwardParserTester.Parse(ForwardParserTester.Create(configuration), template);

            Assert.That(statement.Type, Is.EqualTo(StatementType.Literal));
            Assert.That(statement.Value, Is.EqualTo(expected));
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

        private static ForwardParser Create(DocumentConfiguration configuration = default)
        {
            return new ForwardParser(configuration.BlockBegin ?? "{", configuration.BlockContinue ?? "|",
                configuration.BlockEnd ?? "}", configuration.Escape.GetValueOrDefault('\\'),
                configuration.Trimmer ?? (s => s));
        }

        private static Statement Parse(IParser parser, string template)
        {
            using (var reader = new StringReader(template))
            {
                Assert.That(parser.Parse(reader, out var statement, out _), Is.True);

                return statement;
            }
        }
    }
}