using System;
using Cottle.Documents;
using Cottle.Scopes;
using NUnit.Framework;

namespace Cottle.UTest.Commons
{
	[TestFixture]
	public class CommonFunctionsTester
	{
		[Test]
		[TestCase("{token('A.B.C', '.', 0)}", "A")]
		[TestCase("{token('A//B//C', '//', 1)}", "B")]
		[TestCase("{token('A---B---C', '---', 2)}", "C")]
		[TestCase("{token('A.B.C', '.', 3)}", "")]
		[TestCase("{token('A.B.C', '.', 0, 'XXX')}", "XXX.B.C")]
		[TestCase("{token('A//B//C', '//', 1, 'YYY')}", "A//YYY//C")]
		[TestCase("{token('A---B---C', '---', 2, 'ZZZ')}", "A---B---ZZZ")]
		[TestCase("{token('A______C', '___', 1, 'B')}", "A___B___C")]
		[TestCase("{token('A|B|C', '|', 3, 'D')}", "A|B|C|D")]
		[TestCase("{token('A**B**C**', '**', 3, 'D')}", "A**B**C**D")]
		[TestCase("{token('A---B---C---', '---', 4, 'D')}", "A---B---C------D")]
		public void FunctionToken (string template, string expected)
		{
			CommonFunctionsTester.AssertResult (template, expected);
		}

		private static void AssertResult(string template, string expected)
		{
			IDocument	document = new SimpleDocument (template);
			IScope		scope = new DefaultScope ();

			Assert.AreEqual (expected, document.Render (scope));
		}
	}
}
