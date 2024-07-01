using Cottle.Contexts;
using NUnit.Framework;

namespace Cottle.Test.Builtins
{
    [TestFixture]
    public class BuiltinFunctionsTester
    {
        private static void AssertEqual(string expression, string expected)
        {
            var document = Document.CreateDefault($"{{({expression})=({expected})}}").DocumentOrThrow;

            Assert.That(document.Render(BuiltinContext.Instance), Is.EqualTo("true"), "'{0}' doesn't evaluate to '{1}'",
                expression, expected);
        }

        private static void AssertPrint(string expression, string expected)
        {
            var document = Document.CreateDefault($"{{echo {expression}}}").DocumentOrThrow;

            Assert.That(document.Render(BuiltinContext.Instance), Is.EqualTo(expected), "'{0}' doesn't render to '{1}'",
                expression, expected);
        }

        [Test]
        [TestCase("cast(eq(0, 1), 'n')", "0")]
        [TestCase("cast(eq(0, 0), 'number')", "1")]
        [TestCase("cast(eq(0, 1), 's')", "''")]
        [TestCase("cast(eq(0, 0), 'string')", "'true'")]
        [TestCase("cast(0, 'b')", "eq(0, 1)")]
        [TestCase("cast(1, 'boolean')", "eq(0, 0)")]
        [TestCase("cast(1, 's')", "'1'")]
        [TestCase("cast(1, 'string')", "'1'")]
        [TestCase("cast('0', 'b')", "eq(0, 0)")]
        [TestCase("cast('42', 'b')", "eq(0, 0)")]
        [TestCase("cast('', 'boolean')", "eq(0, 1)")]
        [TestCase("cast('0', 'n')", "0")]
        [TestCase("cast('42', 'n')", "42")]
        [TestCase("cast('ABC', 'number')", "0")]
        public void FunctionCast(string expression, string expected)
        {
            BuiltinFunctionsTester.AssertEqual(expression, expected);
        }

        [Test]
        [TestCase("defined(void)", "")]
        [TestCase("defined(0)", "true")]
        [TestCase("defined(1)", "true")]
        [TestCase("defined('')", "true")]
        [TestCase("defined('A')", "true")]
        [TestCase("defined(eq(0, 0))", "true")]
        [TestCase("defined(eq(0, 1))", "true")]
        [TestCase("defined([])", "true")]
        [TestCase("defined([1])", "true")]
        public void FunctionDefined(string expression, string expected)
        {
            BuiltinFunctionsTester.AssertPrint(expression, expected);
        }

        [Test]
        [TestCase("format(1339936496, 'd:yyyy-MM-dd HH:mm:ss')", "2012-06-17 12:34:56")]
        [TestCase("format(0.165, 'n:e3', 'en-US')", "1.650e-001")]
        [TestCase("format(4.165, 'n:c', 'fr-FR')", "4,17 €")]
        [TestCase("format(1, 'b:n2')", "True")]
        public void FunctionFormat(string expression, string expected)
        {
            BuiltinFunctionsTester.AssertPrint(expression, expected);
        }

        [Test]
        [TestCase("range(3)", "[0, 1, 2]")]
        [TestCase("range(10)", "[0, 1, 2, 3, 4, 5, 6, 7, 8, 9]")]
        [TestCase("range(-2)", "[]")]
        [TestCase("range(0, 5)", "[0, 1, 2, 3, 4]")]
        [TestCase("range(2, 5)", "[2, 3, 4]")]
        [TestCase("range(3, -1)", "[]")]
        [TestCase("range(0, -2, -1)", "[0, -1]")]
        [TestCase("range(3, -4, -1)", "[3, 2, 1, 0, -1, -2, -3]")]
        [TestCase("range(0, 4, 2)", "[0, 2]")]
        [TestCase("range(0, 5, 2)", "[0, 2, 4]")]
        [TestCase("range(5, 20, 5)", "[5, 10, 15]")]
        [TestCase("range(5, 20, 7)", "[5, 12, 19]")]
        [TestCase("range(20, 10, -3)", "[20, 17, 14, 11]")]
        [TestCase("range(20, 10, -5)", "[20, 15]")]
        [TestCase("slice(range(1000000000), 0, 5)", "[0, 1, 2, 3, 4]")]
        public void FunctionRange(string expression, string expected)
        {
            BuiltinFunctionsTester.AssertEqual(expression, expected);
        }

        [Test]
        [TestCase("slice('abc', 0, 5)", "'abc'")]
        [TestCase("slice('abc', 0, 3)", "'abc'")]
        [TestCase("slice('abc', 0, 2)", "'ab'")]
        [TestCase("slice('abc', 0, 0)", "''")]
        [TestCase("slice('abc', 1)", "'bc'")]
        [TestCase("slice('abc', -2)", "'abc'")]
        [TestCase("slice([1, 2, 3], 0, 5)", "[1, 2, 3]")]
        [TestCase("slice([1, 2, 3], 0, 3)", "[1, 2, 3]")]
        [TestCase("slice([1, 2, 3], 0, 2)", "[1, 2]")]
        [TestCase("slice([1, 2, 3], 0, 0)", "[]")]
        [TestCase("slice([1, 2, 3], 1)", "[2, 3]")]
        [TestCase("slice([1, 2, 3], -2)", "[1, 2, 3]")]
        public void FunctionSlice(string expression, string expected)
        {
            BuiltinFunctionsTester.AssertEqual(expression, expected);
        }

        [Test]
        [TestCase("token('A.B.C', '.', 0)", "A")]
        [TestCase("token('A//B//C', '//', 1)", "B")]
        [TestCase("token('A---B---C', '---', 2)", "C")]
        [TestCase("token('A.B.C', '.', 3)", "")]
        [TestCase("token('A.B.C', '.', 0, 'XXX')", "XXX.B.C")]
        [TestCase("token('A//B//C', '//', 1, 'YYY')", "A//YYY//C")]
        [TestCase("token('A---B---C', '---', 2, 'ZZZ')", "A---B---ZZZ")]
        [TestCase("token('A______C', '___', 1, 'B')", "A___B___C")]
        [TestCase("token('A|B|C', '|', 3, 'D')", "A|B|C|D")]
        [TestCase("token('A**B**C**', '**', 3, 'D')", "A**B**C**D")]
        [TestCase("token('A---B---C---', '---', 4, 'D')", "A---B---C------D")]
        public void FunctionToken(string expression, string expected)
        {
            BuiltinFunctionsTester.AssertPrint(expression, expected);
        }

        [Test]
        [TestCase("type(eq(0, 0))", "boolean")]
        [TestCase("type([20, 30])", "map")]
        [TestCase("type(57)", "number")]
        [TestCase("type('A')", "string")]
        [TestCase("type(undefined)", "void")]
        public void FunctionType(string template, string expected)
        {
            BuiltinFunctionsTester.AssertPrint(template, expected);
        }

        [Test]
        [TestCase("xor(0)", "")]
        [TestCase("xor(1)", "true")]
        [TestCase("xor(0, 0)", "")]
        [TestCase("xor(0, 1)", "true")]
        [TestCase("xor(1, 0)", "true")]
        [TestCase("xor(1, 1)", "")]
        [TestCase("xor(0, 0, 0)", "")]
        [TestCase("xor(0, 0, 1)", "true")]
        [TestCase("xor(0, 1, 0)", "true")]
        [TestCase("xor(0, 1, 1)", "")]
        [TestCase("xor(1, 0, 0)", "true")]
        [TestCase("xor(1, 0, 1)", "")]
        [TestCase("xor(1, 1, 0)", "")]
        [TestCase("xor(1, 1, 1)", "")]
        public void FunctionXor(string template, string expected)
        {
            BuiltinFunctionsTester.AssertPrint(template, expected);
        }
    }
}