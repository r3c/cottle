using System;
using System.Collections.Generic;
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
		public void EchoConstant (string expression, string expected)
		{
			this.AssertRender ("{echo " + expression + "}", expected);
		}

		[Test]
		[TestCase ("aaa[0]", "5")]
		[TestCase ("aaa[1]", "7")]
		[TestCase ("aaa[2]", "")]
		[TestCase ("bbb.x", "$X$")]
		[TestCase ("bbb[\"y\"]", "$Y$")]
		[TestCase ("bbb.z", "")]
		[TestCase ("ccc", "")]
		public void EchoValueAccess (string access, string expected)
		{
			IScope	scope;

			scope = new SimpleScope ();
			scope["aaa"] = new [] { (Value)5, (Value)7 };
			scope["bbb"] = new Dictionary<Value, Value>
			{
				{"x",	"$X$"},
				{"y",	"$Y$"}
			};

			this.AssertRender ("{echo " + access + "}", expected, DefaultSetting.Instance, scope);
		}

		[Test]
		[TestCase ("aaa", "I sense a soul")]
		[TestCase ("_", "in search of answers")]
		[TestCase ("missing", "")]
		public void EchoValueDirect (string name, string expected)
		{
			IScope	scope;

			scope = new SimpleScope ();
			scope["aaa"] = "I sense a soul";
			scope["_"] = "in search of answers";

			this.AssertRender ("{echo " + name + "}", expected, DefaultSetting.Instance, scope);
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

		private void AssertRender (string source, string expected, ISetting setting, IScope constant)
		{
			IDocument	document;
			IScope		scope;

			foreach (Func<string, ISetting, IDocument> constructor in DocumentTester.constructors)
			{
				document = constructor (source, setting);
				scope = new FallbackScope (constant, new SimpleScope ());

				Assert.AreEqual (expected, document.Render (scope), "Rendering failed for document type '{0}'", document.GetType ());
			}
		}

		private void AssertRender (string source, string expected)
		{
			this.AssertRender (source, expected, DefaultSetting.Instance, new SimpleScope ());
		}
	}
}
