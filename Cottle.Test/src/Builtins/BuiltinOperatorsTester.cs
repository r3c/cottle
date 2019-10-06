using Cottle.Contexts;
using Cottle.Documents;
using NUnit.Framework;

namespace Cottle.Test.Builtins
{
    [TestFixture]
    public class BuiltinOperatorsTester
    {
        private static void AssertResult(string expression, bool expected)
        {
            var document = new SimpleDocument("{" + (expected ? "" : "!") + "(" + expression + ")}");

            Assert.That(document.Render(BuiltinContext.Instance), Is.EqualTo("true"));
        }

        [Test]
        [TestCase("0 + 0", "0")]
        [TestCase("0 + 1.5", "1.5")]
        [TestCase("3 + 0", "3")]
        [TestCase("1 + 2 + 3", "6")]
        [TestCase("+1 + 2", "3")]
        public void OperatorAdd(string expression, string expected)
        {
            BuiltinOperatorsTester.AssertResult(expression + "=" + expected, true);
        }

        [Test]
        [TestCase("0 && 0", false)]
        [TestCase("0 && 1", false)]
        [TestCase("1 && 0", false)]
        [TestCase("1 && 1", true)]
        public void OperatorAnd(string expression, bool expected)
        {
            BuiltinOperatorsTester.AssertResult(expression, expected);
        }

        [Test]
        [TestCase("0 / 0", null)]
        [TestCase("1 / 0", null)]
        [TestCase("3 / 3", "1")]
        [TestCase("18 / 3 / 2", "3")]
        public void OperatorDiv(string expression, string expected)
        {
            BuiltinOperatorsTester.AssertResult(expression + "=" + (expected ?? "0%0"), true);
        }

        [Test]
        [TestCase("0", "0", true)]
        [TestCase("0", "1", false)]
        [TestCase("'Hello'", "'World'", false)]
        [TestCase("'Equal'", "'Equal'", true)]
        public void OperatorEqualNotEqual(string lhs, string rhs, bool expected)
        {
            BuiltinOperatorsTester.AssertResult(lhs + "=" + rhs, expected);
            BuiltinOperatorsTester.AssertResult(lhs + "!=" + rhs, !expected);
        }

        [Test]
        [TestCase("0", "0", false)]
        [TestCase("0", "1", true)]
        [TestCase("2", "1", false)]
        [TestCase("'AX'", "'AX'", false)]
        [TestCase("'AX'", "'BX'", true)]
        [TestCase("'BX'", "'AX'", false)]
        public void OperatorGreaterEqualLowerThan(string lhs, string rhs, bool expected)
        {
            BuiltinOperatorsTester.AssertResult(lhs + ">=" + rhs, !expected);
            BuiltinOperatorsTester.AssertResult(lhs + "<" + rhs, expected);
        }

        [Test]
        [TestCase("0", "0", true)]
        [TestCase("0", "1", true)]
        [TestCase("2", "1", false)]
        [TestCase("'AX'", "'AX'", true)]
        [TestCase("'AX'", "'BX'", true)]
        [TestCase("'BX'", "'AX'", false)]
        public void OperatorGreaterThanLowerEqual(string lhs, string rhs, bool expected)
        {
            BuiltinOperatorsTester.AssertResult(lhs + ">" + rhs, !expected);
            BuiltinOperatorsTester.AssertResult(lhs + "<=" + rhs, expected);
        }

        [Test]
        [TestCase("0 % 0", null)]
        [TestCase("1 % 0", null)]
        [TestCase("3 % 3", "0")]
        [TestCase("18 % 5 % 2", "1")]
        public void OperatorMod(string expression, string expected)
        {
            BuiltinOperatorsTester.AssertResult(expression + "=" + (expected ?? "0/0"), true);
        }

        [Test]
        [TestCase("0 * 0", "0")]
        [TestCase("0 * 1.5", "0")]
        [TestCase("3 * 1", "3")]
        [TestCase("3 * 2 * 1", "6")]
        public void OperatorMul(string expression, string expected)
        {
            BuiltinOperatorsTester.AssertResult(expression + "=" + expected, true);
        }

        [Test]
        [TestCase("!0", true)]
        [TestCase("!1", false)]
        public void OperatorNot(string expression, bool expected)
        {
            BuiltinOperatorsTester.AssertResult(expression, expected);
        }

        [Test]
        [TestCase("0 || 0", false)]
        [TestCase("0 || 1", true)]
        [TestCase("1 || 0", true)]
        [TestCase("1 || 1", true)]
        public void OperatorOr(string expression, bool expected)
        {
            BuiltinOperatorsTester.AssertResult(expression, expected);
        }

        [Test]
        [TestCase("0 - 0", "0")]
        [TestCase("0 - 1.5", "-1.5")]
        [TestCase("3 - 0", "3")]
        [TestCase("3 - 2 - 1", "0")]
        [TestCase("-1 - 2", "-3")]
        public void OperatorSub(string expression, string expected)
        {
            BuiltinOperatorsTester.AssertResult(expression + "=" + expected, true);
        }

        [Test]
        [TestCase("2 - 1 - 1", "0")]
        [TestCase("2 - (1 - 1)", "2")]
        [TestCase("(2 - 1) - 1", "0")]
        [TestCase("1 + 2 * 3", "7")]
        [TestCase("1 * 2 + 3", "5")]
        public void Precedence(string expression, string expected)
        {
            BuiltinOperatorsTester.AssertResult(expression + "=" + expected, true);
        }
    }
}