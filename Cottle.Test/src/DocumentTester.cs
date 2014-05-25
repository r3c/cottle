using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Cottle.Documents;
using Cottle.Scopes;
using Cottle.Settings;
using NUnit.Framework;

namespace Cottle.Test
{
	[TestFixture]
	public class DocumentTester
	{
		public static readonly Func<string, ISetting, IDocument>[]	constructors =
		{
			(source, setting) => new DynamicDocument (source, setting),
			(source, setting) => new SimpleDocument (source, setting)
		};

		[Test]
		[TestCase ("5", "5")]
		[TestCase ("\"Hello, World!\"", "Hello, World!")]
		public void CommandEcho (string value, string expected)
		{
			this.AssertRender ("{echo " + value + "}", expected);
		}

		[Test]
		[TestCase ("1", "1")]
		[TestCase ("'A'", "\"A\"")]
		[TestCase ("[]", "[]")]
		public void CommandReturn (string value, string expected)
		{
			this.AssertReturn ("{return " + value + "}", expected);
		}

		[Test]
		public void TextEmpty ()
		{
			this.AssertRender (string.Empty, string.Empty);
		}

		[Test]
		[TestCase ("\\\\", "\\")]
		[TestCase ("\\{\\|\\}", "{|}")]
		[TestCase ("a\\{b\\|c\\}d", "a{b|c}d")]
		public void TextEscape (string escaped, string expected)
		{
			this.AssertRender (escaped, expected);
		}

		[Test]
		[TestCase ("Hello, World!")]
		[TestCase ("This is some literal text")]
		public void TextLiteral (string expected)
		{
			this.AssertRender (expected, expected);
		}

		[Test]
		[TestCase ("A", "A", "B", "B")]
		[TestCase ("X  Y", " +", " ", "X Y")]
		[TestCase ("df98gd76dfg5df4g321gh0", "[^0-9]", "", "9876543210")]
		public void TextTrim (string value, string pattern, string replacement, string expected)
		{
			CustomSetting	setting;

			setting = new CustomSetting ();
			setting.Trimmer = (s) => Regex.Replace (s, pattern, replacement);

			this.AssertRender (value, expected, setting, new SimpleScope ());
		}

		[Test]
		[TestCase ("aaa[0]", "5")]
		[TestCase ("aaa[1]", "7")]
		[TestCase ("aaa[2]", "<void>")]
		[TestCase ("bbb.x", "\"$X$\"")]
		[TestCase ("bbb[\"y\"]", "\"$Y$\"")]
		[TestCase ("bbb.z", "<void>")]
		[TestCase ("ccc.A.i", "50")]
		[TestCase ("ccc.A['i']", "50")]
		[TestCase ("ccc['A'].i", "50")]
		[TestCase ("ccc['A']['i']", "50")]
		[TestCase ("ccc[1]", "42")]
		[TestCase ("ccc['1']", "<void>")]
		[TestCase ("ddd", "<void>")]
		public void ValueAccess (string access, string expected)
		{
			IScope	scope;

			scope = new SimpleScope ();
			scope["aaa"] = new [] { (Value)5, (Value)7 };
			scope["bbb"] = new Dictionary<Value, Value>
			{
				{"x",	"$X$"},
				{"y",	"$Y$"}
			};
			scope["ccc"] = new Dictionary<Value, Value>
			{
				{"A",	new Dictionary<Value, Value>
				{
					{"i",	50}
				}},
				{1,		42}
			};

			this.AssertReturn ("{return " + access + "}", expected, DefaultSetting.Instance, scope);
		}

		[Test]
		[TestCase ("42", "42")]
		[TestCase ("-17.2", "-17.2")]
		[TestCase ("\"42\"", "\"42\"")]
		[TestCase ("\"ABC\"", "\"ABC\"")]
		public void ValueConstant (string constant, string expected)
		{
			this.AssertReturn ("{return " + constant + "}", expected);
		}

		[Test]
		[TestCase ("[][0]", "<void>")]
		[TestCase ("[0][0]", "0")]
		[TestCase ("[5][0]", "5")]
		[TestCase ("[5][1]", "<void>")]
		[TestCase ("[5, 8, 2][1]", "8")]
		[TestCase ("[5, 2, 'A', 2][2]", "\"A\"")]
		[TestCase ("[2: 'A', 5: 'B'][0]", "<void>")]
		[TestCase ("[2: 'A', 5: 'B'][2]", "\"A\"")]
		[TestCase ("[2: 'A', 5: 'B']['2']", "<void>")]
		[TestCase ("['x': 'X', 'y': 'Y']['y']", "\"Y\"")]
		[TestCase ("['a': ['b': ['c': 42]]]['a']['b']['c']", "42")]
		public void ValueMap (string expression, string expected)
		{
			this.AssertReturn ("{return " + expression + "}", expected);
		}

		[Test]
		[TestCase ("aaa", "\"I sense a soul\"")]
		[TestCase ("_", "\"in search of answers\"")]
		[TestCase ("missing", "<void>")]
		public void ValueSymbol (string symbol, string expected)
		{
			IScope	scope;

			scope = new SimpleScope ();
			scope["aaa"] = "I sense a soul";
			scope["_"] = "in search of answers";

			this.AssertReturn ("{return " + symbol + "}", expected, DefaultSetting.Instance, scope);
		}

		private void AssertRender (string source, string expected, ISetting setting, IScope constant)
		{
			IDocument	document;
			IScope		scope;

			foreach (Func<string, ISetting, IDocument> constructor in DocumentTester.constructors)
			{
				document = constructor (source, setting);
				scope = new FallbackScope (constant, new SimpleScope ());

				Assert.AreEqual (expected, document.Render (scope), "Invalid rendered output for document type '{0}'", document.GetType ());
			}
		}

		private void AssertRender (string source, string expected)
		{
			this.AssertRender (source, expected, DefaultSetting.Instance, new SimpleScope ());
		}

		private void AssertReturn (string source, string expected, ISetting setting, IScope constant)
		{
			IDocument	document;
			IScope		scope;
			Value		value;

			foreach (Func<string, ISetting, IDocument> constructor in DocumentTester.constructors)
			{
				document = constructor (source, setting);
				scope = new FallbackScope (constant, new SimpleScope ());
				value = document.Render (scope, new StringWriter ());

				Assert.AreEqual (expected, value.ToString (), "Invalid return value for document type '{0}'", document.GetType ());
			}
		}

		private void AssertReturn (string source, string expected)
		{
			this.AssertReturn (source, expected, DefaultSetting.Instance, new SimpleScope ());
		}
	}
}
