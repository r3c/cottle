using System;
using Cottle.Documents;
using Cottle.Stores;
using NUnit.Framework;

namespace Cottle.Test.Builtins
{
	[TestFixture]
	public class BuiltinOperatorsTester
	{
		[Test]
		[TestCase ("0 + 0", "0")]
		[TestCase ("0 + 1.5", "1.5")]
		[TestCase ("3 + 0", "3")]
		[TestCase ("1 + 2 + 3", "6")]
		public void OperatorAdd (string expression, string expected)
		{
			BuiltinOperatorsTester.AssertTrue (expression + "=" + expected);
		}

		[Test]
		[TestCase ("0 / 0", null)]
		[TestCase ("1 / 0", null)]
		[TestCase ("3 / 3", "1")]
		[TestCase ("18 / 3 / 2", "3")]
		public void OperatorDiv (string expression, string expected)
		{
			BuiltinOperatorsTester.AssertTrue (expression + "=" + (expected ?? "0%0"));
		}

		[Test]
		[TestCase ("0", "0", true)]
		[TestCase ("0", "1", false)]
		[TestCase ("'Hello'", "'World'", false)]
		[TestCase ("'Equal'", "'Equal'", true)]
		public void OperatorEqualNotEqual (string lhs, string rhs, bool expected)
		{
			BuiltinOperatorsTester.AssertTrue ((expected ? "" : "!") + "(" + lhs + "=" + rhs + ")");
			BuiltinOperatorsTester.AssertTrue ((expected ? "!" : "") + "(" + lhs + "!=" + rhs + ")");
		}

		[Test]
		[TestCase ("0", "0", true)]
		[TestCase ("0", "1", true)]
		[TestCase ("2", "1", false)]
		[TestCase ("'AX'", "'AX'", true)]
		[TestCase ("'AX'", "'BX'", true)]
		[TestCase ("'BX'", "'AX'", false)]
		public void OperatorGreaterThanLowerEqual (string lhs, string rhs, bool expected)
		{
			BuiltinOperatorsTester.AssertTrue ((expected ? "!" : "") + "(" + lhs + ">" + rhs + ")");
			BuiltinOperatorsTester.AssertTrue ((expected ? "" : "!") + "(" + lhs + "<=" + rhs + ")");
		}

		[Test]
		[TestCase ("0", "0", false)]
		[TestCase ("0", "1", true)]
		[TestCase ("2", "1", false)]
		[TestCase ("'AX'", "'AX'", false)]
		[TestCase ("'AX'", "'BX'", true)]
		[TestCase ("'BX'", "'AX'", false)]
		public void OperatorGreaterEqualLowerThan (string lhs, string rhs, bool expected)
		{
			BuiltinOperatorsTester.AssertTrue ((expected ? "!" : "") + "(" + lhs + ">=" + rhs + ")");
			BuiltinOperatorsTester.AssertTrue ((expected ? "" : "!") + "(" + lhs + "<" + rhs + ")");
		}

		[Test]
		[TestCase ("0 % 0", null)]
		[TestCase ("1 % 0", null)]
		[TestCase ("3 % 3", "0")]
		[TestCase ("18 % 5 % 2", "1")]
		public void OperatorMod (string expression, string expected)
		{
			BuiltinOperatorsTester.AssertTrue (expression + "=" + (expected ?? "0/0"));
		}

		[Test]
		[TestCase ("0 * 0", "0")]
		[TestCase ("0 * 1.5", "0")]
		[TestCase ("3 * 1", "3")]
		[TestCase ("3 * 2 * 1", "6")]
		public void OperatorMul (string expression, string expected)
		{
			BuiltinOperatorsTester.AssertTrue (expression + "=" + expected);
		}

		[Test]
		[TestCase ("!0", true)]
		[TestCase ("!1", false)]
		public void OperatorNot (string expression, bool expected)
		{
			BuiltinOperatorsTester.AssertTrue ((expected ? "" : "!") + "(" + expression + ")");
		}

		[Test]
		[TestCase ("0 - 0", "0")]
		[TestCase ("0 - 1.5", "-1.5")]
		[TestCase ("3 - 0", "3")]
		[TestCase ("3 - 2 - 1", "0")]
		public void OperatorSub (string expression, string expected)
		{
			BuiltinOperatorsTester.AssertTrue (expression + "=" + expected);
		}

		[Test]
		[TestCase ("2 - 1 - 1", "0")]
		[TestCase ("2 - (1 - 1)", "2")]
		[TestCase ("(2 - 1) - 1", "0")]
		[TestCase ("1 + 2 * 3", "7")]
		[TestCase ("1 * 2 + 3", "5")]
		public void Precedence (string expression, string expected)
		{
			BuiltinOperatorsTester.AssertTrue (expression + "=" + expected);
		}

		private static void AssertTrue (string expression)
		{
			IDocument	document = new SimpleDocument ("{" + expression + "}");
			IStore		store = new BuiltinStore ();

			Assert.AreEqual ("true", document.Render (store));
		}
	}
}
