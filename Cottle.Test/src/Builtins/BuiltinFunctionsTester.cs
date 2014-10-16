using System;
using Cottle.Documents;
using Cottle.Stores;
using NUnit.Framework;

namespace Cottle.Test.Builtins
{
	[TestFixture]
	public class BuiltinFunctionsTester
	{
		[Test]
		[TestCase ("range(3)", "[0, 1, 2]")]
		[TestCase ("range(10)", "[0, 1, 2, 3, 4, 5, 6, 7, 8, 9]")]
		[TestCase ("range(-2)", "[]")]
		[TestCase ("range(0, 5)", "[0, 1, 2, 3, 4]")]
		[TestCase ("range(2, 5)", "[2, 3, 4]")]
		[TestCase ("range(3, -1)", "[]")]
		[TestCase ("range(0, -2, -1)", "[0, -1]")]
		[TestCase ("range(3, -4, -1)", "[3, 2, 1, 0, -1, -2, -3]")]
		[TestCase ("range(0, 4, 2)", "[0, 2]")]
		[TestCase ("range(0, 5, 2)", "[0, 2, 4]")]
		[TestCase ("range(5, 20, 5)", "[5, 10, 15]")]
		[TestCase ("range(5, 20, 7)", "[5, 12, 19]")]
		[TestCase ("range(20, 10, -3)", "[20, 17, 14, 11]")]
		[TestCase ("range(20, 10, -5)", "[20, 15]")]
		[TestCase ("slice(range(1000000000), 0, 5)", "[0, 1, 2, 3, 4]")]
		public void FunctionRange (string expression, string expected)
		{
			BuiltinFunctionsTester.AssertEqual (expression, expected);
		}

		[Test]
		[TestCase ("slice('abc', 0, 5)", "'abc'")]
		[TestCase ("slice('abc', 0, 3)", "'abc'")]
		[TestCase ("slice('abc', 0, 2)", "'ab'")]
		[TestCase ("slice('abc', 0, 0)", "''")]
		[TestCase ("slice('abc', 1)", "'bc'")]
		[TestCase ("slice('abc', -2)", "'abc'")]
		[TestCase ("slice([1, 2, 3], 0, 5)", "[1, 2, 3]")]
		[TestCase ("slice([1, 2, 3], 0, 3)", "[1, 2, 3]")]
		[TestCase ("slice([1, 2, 3], 0, 2)", "[1, 2]")]
		[TestCase ("slice([1, 2, 3], 0, 0)", "[]")]
		[TestCase ("slice([1, 2, 3], 1)", "[2, 3]")]
		[TestCase ("slice([1, 2, 3], -2)", "[1, 2, 3]")]
		public void FunctionSlice (string expression, string expected)
		{
			BuiltinFunctionsTester.AssertEqual (expression, expected);
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
		public void FunctionToken (string template, string expected)
		{
			BuiltinFunctionsTester.AssertPrint (template, expected);
		}

		private static void AssertEqual (string expression, string expected)
		{
			IDocument	document = new SimpleDocument ("{eq(" + expression + ", " + expected + ")}");
			IStore		store = new BuiltinStore ();

			Assert.AreEqual ("true", document.Render (store));
		}

		private static void AssertPrint (string template, string expected)
		{
			IDocument	document = new SimpleDocument ("{echo " + template + "}");
			IStore		store = new BuiltinStore ();

			Assert.AreEqual (expected, document.Render (store));
		}
	}
}
