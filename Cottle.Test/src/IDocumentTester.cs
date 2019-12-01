using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Cottle.Builtins;
using Cottle.Documents;
using Cottle.Functions;
using Cottle.Stores;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test
{
    [TestFixture]
    // ReSharper disable once InconsistentNaming
    public abstract class IDocumentTester
    {
        [Test]
        [TestCase("<|", "|>", "1 || 0", "true")]
        [TestCase("<&", "&>", "1 && 0", "")]
        public void ConfigurationBlockAmbiguous(string blockBegin, string blockEnd, string expression, string expected)
        {
            var configuration = new DocumentConfiguration
                { BlockBegin = blockBegin, BlockContinue = "<>", BlockEnd = blockEnd };

            AssertRender(blockBegin + expression + blockEnd, configuration, Context.Empty, expected);
        }

        [Test]
        [TestCase("{\"\\\"\"}", '\\', "\"")]
        [TestCase("{\"xxxy\"}", 'x', "xy")]
        public void ConfigurationEscapeCommand(string input, char escape, string expected)
        {
            var configuration = new DocumentConfiguration { Escape = escape };

            AssertRender(input, configuration, Context.Empty, expected);
        }

        [Test]
        [TestCase("Escaped\\ Literal", '\\', "Escaped Literal")]
        [TestCase("-{'x'-}", '-', "{'x'}")]
        [TestCase("ABC", 'x', "ABC")]
        [TestCase("--", '-', "-")]
        public void ConfigurationEscapeText(string input, char escape, string expected)
        {
            var configuration = new DocumentConfiguration { Escape = escape };

            AssertRender(input, configuration, Context.Empty, expected);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ConfigurationOptimizeConstantMap(bool noOptimize)
        {
            var configuration = new DocumentConfiguration { NoOptimize = noOptimize };

            AssertRender("{['X', 'Y', 'Z'][0]}", configuration, Context.Empty, "X");
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void ConfigurationOptimizeReturn(bool noOptimize)
        {
            var configuration = new DocumentConfiguration { NoOptimize = noOptimize };

            AssertRender("X{return 1}Y", configuration, Context.Empty, "X");
        }

        [Test]
        [TestCase("A", "A", "B", "B")]
        [TestCase("X  Y", " +", " ", "X Y")]
        [TestCase("df98gd76dfg5df4g321gh0", "[^0-9]", "", "9876543210")]
        public void ConfigurationTrimmer(string value, string pattern, string replacement, string expected)
        {
            var configuration = new DocumentConfiguration { Trimmer = s => Regex.Replace(s, pattern, replacement) };

            AssertRender(value, configuration, Context.Empty, expected);
        }

        [Test]
        public void RenderCommandDeclare()
        {
            AssertRender("{declare var}", string.Empty);
        }

        [Test]
        [TestCase("expression", "{3}", "3")]
        [TestCase("mixed", "A{'B'}C", "ABC")]
        [TestCase("number", "1", "1")]
        [TestCase("text", "something", "something")]
        public void RenderCommandDeclareRender(string name, string body, string expected)
        {
            AssertRender("{declare " + name + " as:" + body + "}{echo " + name + "}", expected);
        }

        [Test]
        [TestCase("var", "1", "1")]
        [TestCase("_", "'A'", "\"A\"")]
        [TestCase("some_symbol_name", "[]", "[]")]
        public void CommandDeclareValueScope(string name, string value, string expected)
        {
            AssertReturn(
                "{declare f() as:{declare " + name + " as 'unused'}}{declare " + name + " as " + value +
                "}{f()}{return " + name + "}", expected);
        }

        [Test]
        [TestCase("var", "1", "1")]
        [TestCase("_", "'A'", "\"A\"")]
        [TestCase("some_symbol_name", "[]", "[]")]
        public void RenderCommandDeclareValueSimple(string name, string value, string expected)
        {
            AssertReturn("{declare " + name + " as " + value + "}{return " + name + "}", expected);
        }

        [Test]
        [TestCase("5", "5")]
        [TestCase("\"Hello, World!\"", "Hello, World!")]
        public void RenderCommandEcho(string value, string expected)
        {
            AssertRender("{echo " + value + "}", expected);
        }

        [Test]
        [TestCase("k", "v", "[]", "-", "EMPTY")]
        [TestCase("key", "value", "['A': 'X', 'B': 'Y', 'C': 'Z']", "{key}{value}", "AXBYCZ")]
        [TestCase("i", "j", "[1, 5, 9]", "{i}{j}", "011529")]
        public void RenderCommandForKeyValue(string name1, string name2, string source, string body, string expected)
        {
            AssertRender("{for " + name1 + ", " + name2 + " in " + source + ":" + body + "|empty:EMPTY}", expected);
        }

        [Test]
        [TestCase("unused", "[]", "-", "EMPTY")]
        [TestCase("dummy", "[1, 5, 9]", "X", "XXX")]
        [TestCase("v", "[1, 5, 9]", "{v}", "159")]
        [TestCase("name", "[5: 'A', 9: 'B', 2: 'C']", "{name}", "ABC")]
        public void RenderCommandForValue(string name, string source, string body, string expected)
        {
            AssertRender("{for " + name + " in " + source + ":" + body + "|empty:EMPTY}", expected);
        }

        [Test]
        [TestCase("1", "true", "true")]
        [TestCase("''", "invisible", "")]
        [TestCase("'something'", "visible", "visible")]
        [TestCase("[]", "invisible", "")]
        [TestCase("[1, 2, 3]", "visible", "visible")]
        [TestCase("1", "a|elif 1:b|else:c", "a")]
        [TestCase("0", "a|elif 1:b|else:c", "b")]
        [TestCase("0", "a|elif 0:b|else:c", "c")]
        [TestCase("1 + 0", "a|elif 1 + 0:b|else:c", "a")]
        [TestCase("0 + 0", "a|elif 1 + 0:b|else:c", "b")]
        [TestCase("0 + 0", "a|elif 0 + 0:b|else:c", "c")]
        public void RenderCommandIf(string condition, string body, string expected)
        {
            AssertRender("{if " + condition + ":" + body + "}", expected);
        }

        [Test]
        [TestCase("1", "1")]
        [TestCase("'A'", "\"A\"")]
        [TestCase("[]", "[]")]
        public void RenderCommandReturnDirect(string value, string expected)
        {
            AssertReturn("{return " + value + "}", expected);
        }

        [Test]
        [TestCase("0", "0", "\"B\"")]
        [TestCase("0", "1", "\"B\"")]
        [TestCase("1", "0", "\"AB\"")]
        [TestCase("1", "1", "\"AA\"")]
        public void RenderCommandReturnNested(string a, string b, string expected)
        {
            AssertReturn("{if " + a + ":{if " + b + ":{return 'AA'}|else:{return 'AB'}}|else:{return 'B'}}", expected);
        }

        [Test]
        public void RenderCommandSet()
        {
            AssertRender("{set var}", string.Empty);
        }

        [Test]
        [TestCase("{set a to 1}{set b to 2}", "", "{a}{b}", "12")]
        [TestCase("{set a to 1}{set b to 2}", "{a}{b}", "{a}{b}", "3412")]
        public void RenderCommandSetFunctionArguments(string pre, string body, string post, string expected)
        {
            AssertRender(pre + "{set f(a, b) to:" + body + "}{f(3, 4)}" + post, expected);
        }

        [Test]
        [TestCase("f", "Hello, World!", "Hello, World!")]
        [TestCase("test", "{'Some String'}", "Some String")]
        public void RenderCommandSetFunctionRender(string name, string body, string expected)
        {
            AssertRender("{set " + name + "() to:" + body + "}{return " + name + "()}", expected);
        }

        [Test]
        [TestCase("f", "{return 1}", "1")]
        [TestCase("test", "{return 'Some String'}", "\"Some String\"")]
        [TestCase("x", "{if 1:{return 'X'}|else:{return 'Y'}}", "\"X\"")]
        public void RenderCommandSetFunctionReturn(string name, string body, string expected)
        {
            AssertReturn("{set " + name + "(a) to:" + body + "}{return " + name + "()}", expected);
        }

        [Test]
        [TestCase("{while parent:{parent.value}{set parent to parent.child}}")]
        public void RenderCommandSetValueInLoop(string source)
        {
            AssertRender(source, default, Context.CreateCustom(
                new Dictionary<Value, Value>
                {
                    ["parent"] = new Dictionary<Value, Value>
                        { ["child"] = new Dictionary<Value, Value> { ["value"] = "53" }, ["value"] = "42" }
                }), "4253");
        }

        [Test]
        [TestCase("var", "1", "1")]
        [TestCase("_", "'A'", "\"A\"")]
        [TestCase("some_symbol_name", "[]", "[]")]
        public void RenderCommandSetValueScope(string name, string value, string expected)
        {
            AssertReturn(
                "{declare f() as:{set " + name + " to " + value + "}}{set " + name + " to 'default'}{f()}{return " +
                name + "}", expected);
        }

        [Test]
        [TestCase("var", "1", "1")]
        [TestCase("_", "'A'", "\"A\"")]
        [TestCase("some_symbol_name", "[]", "[]")]
        public void RenderCommandSetValueSimple(string name, string value, string expected)
        {
            AssertReturn("{set " + name + " to " + value + "}{return " + name + "}", expected);
        }

        [Test]
        [TestCase("{set a to 0}", "lt(a, 8)", "{set a to add(a, 1)}{a}", "12345678")]
        [TestCase("{set a to 8}", "lt(0, a)", "{set a to add(a, -1)}X", "XXXXXXXX")]
        public void RenderCommandWhile(string init, string condition, string body, string expected)
        {
            AssertRender(init + "{while " + condition + ":" + body + "}", default,
                IDocumentTester.CreateContextWithBuiltins("add", "lt"), expected);
        }

        [Test]
        [TestCase("Hello, World!")]
        [TestCase("This is some literal text")]
        public void RenderLiteral(string expected)
        {
            AssertRender(expected, expected);
        }

        [Test]
        public void RenderLiteralEmpty()
        {
            AssertRender(string.Empty, string.Empty);
        }

        [Test]
        [TestCase("\\\\", "\\")]
        [TestCase("\\{\\|\\}", "{|}")]
        [TestCase("a\\{b\\|c\\}d", "a{b|c}d")]
        public void RenderLiteralEscape(string escaped, string expected)
        {
            AssertRender(escaped, expected);
        }

        [Test]
        [TestCase("aaa[0]", "5")]
        [TestCase("aaa[1]", "7")]
        [TestCase("aaa[2]", "<void>")]
        [TestCase("bbb.x", "\"$X$\"")]
        [TestCase("bbb[\"y\"]", "\"$Y$\"")]
        [TestCase("bbb.z", "<void>")]
        [TestCase("ccc.A.i", "50")]
        [TestCase("ccc.A['i']", "50")]
        [TestCase("ccc['A'].i", "50")]
        [TestCase("ccc['A']['i']", "50")]
        [TestCase("ccc[1]", "42")]
        [TestCase("ccc['1']", "<void>")]
        [TestCase("ddd", "<void>")]
        public void RenderExpressionAccess(string access, string expected)
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                { "aaa", new[] { 5, (Value)7 } },
                { "bbb", new Dictionary<Value, Value> { { "x", "$X$" }, { "y", "$Y$" } } },
                {
                    "ccc", new Dictionary<Value, Value>
                        { { "A", new Dictionary<Value, Value> { { "i", 50 } } }, { 1, 42 } }
                }
            });

            AssertReturn("{return " + access + "}", default, context, expected);
        }

        [Test]
        [TestCase("42", "42")]
        [TestCase("-17.2", "-17.2")]
        [TestCase("\"42\"", "\"42\"")]
        [TestCase("\"ABC\"", "\"ABC\"")]
        public void RenderExpressionConstant(string constant, string expected)
        {
            AssertReturn("{return " + constant + "}", expected);
        }

        [Test]
        [TestCase("abc", 42)]
        [TestCase("xyz", 17)]
        public void RenderExpressionInvoke(string symbol, int expected)
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                { symbol, expected },
                {
                    "f", new NativeFunction((a, s, o) =>
                    {
                        var value = s[a[0]];

                        o.Write(value.AsString);

                        return value;
                    }, 1)
                }
            });

            AssertRender("{f('" + symbol + "')}",
                default, context, ((Value)expected).AsString + ((Value)expected).AsString);
            AssertReturn("{return f('" + symbol + "')}",
                default, context, expected.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        [TestCase("[][0]", "<void>")]
        [TestCase("[0][0]", "0")]
        [TestCase("[5][0]", "5")]
        [TestCase("[5][1]", "<void>")]
        [TestCase("[5, 8, 2][1]", "8")]
        [TestCase("[5, 2, 'A', 2][2]", "\"A\"")]
        [TestCase("[2: 'A', 5: 'B'][0]", "<void>")]
        [TestCase("[2: 'A', 5: 'B'][2]", "\"A\"")]
        [TestCase("[2: 'A', 5: 'B']['2']", "<void>")]
        [TestCase("['x': 'X', 'y': 'Y']['y']", "\"Y\"")]
        [TestCase("['a': ['b': ['c': 42]]]['a']['b']['c']", "42")]
        public void RenderExpressionMap(string expression, string expected)
        {
            AssertReturn("{return " + expression + "}", expected);
        }

        [Test]
        [TestCase("aaa", "aaa", "I sense a soul", "\"I sense a soul\"")]
        [TestCase("_", "_", "in search of answers", "\"in search of answers\"")]
        [TestCase("x", "missing", "x", "<void>")]
        public void RenderExpressionSymbol(string set, string get, string value, string expected)
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                { set, value }
            });

            AssertReturn("{return " + get + "}", default, context, expected);
        }

        [Test]
        [TestCase("1", "1")]
        [TestCase("3", "6")]
        [TestCase("8", "40320")]
        public void SampleFactorial(string value, string expected)
        {
            AssertReturn(@"
                {set factorial(n) to:
                    {if n > 1:
                        {return n * factorial(n - 1)}
                    |else:
                        {return 1}
                    }
                }
                {return factorial(" + value + ")}",
                new DocumentConfiguration { Trimmer = DocumentConfiguration.TrimIndentCharacters }, Context.Empty,
                expected);
        }

        [Test]
        [TestCase("2", "A -> B, A -> C, B -> C, ")]
        [TestCase("3", "A -> C, A -> B, C -> B, A -> C, B -> A, B -> C, A -> C, ")]
        public void SampleHanoiTowers(string disks, string expected)
        {
            AssertRender(@"
                {set hanoi_rec(n, from, by, to) to:
                    {set n to n - 1}
                    {if n > 0:
                        {hanoi_rec(n, from, to, by)}
                    }
                    {from} -> {to}, 
                    {if n > 0:
                        {hanoi_rec(n, by, from, to)}
                    }
                }
                {set hanoi(n) to:
                    {hanoi_rec(n, 'A', 'B', 'C')}
                }
                {hanoi(" + disks + ")}",
                new DocumentConfiguration { Trimmer = DocumentConfiguration.TrimIndentCharacters }, Context.Empty,
                expected);
        }

        [Test]
        [TestCase("2", "4")]
        [TestCase("8", "256")]
        [TestCase("16", "65536")]
        public void SamplePowerOfTwo(string value, string expected)
        {
            AssertReturn(@"
                {set a to " + value + @"}
                {set b to 1}
                {while a > 0:
                    {set a to a - 1}
                    {set b to b + b}
                }
                {return b}
            ", new DocumentConfiguration { Trimmer = DocumentConfiguration.TrimIndentCharacters }, Context.Empty,
                expected);
        }

        [Test]
        [TestCase("", 45000)]
        [TestCase("", 90000)]
        [TestCase("", 200000)]
        [TestCase("{foo}", 45000)]
        [TestCase("{foo}", 90000)]
        [TestCase("{foo}", 200000)]
        public void TextTooLong(string variable, int characters)
        {
            var builder = new StringBuilder();
            var store = new SimpleStore();

            // Add some variables in the text to test a use case where we don't have only text
            if (!string.IsNullOrEmpty(variable))
                builder.Append(variable);

            builder.Append('a', characters);

            // Add some variables in the text to test a use case where we don't have only text
            if (!string.IsNullOrEmpty(variable))
                builder.Append(variable);

            var input = builder.ToString();

            store["foo"] = "{foo}";

            var document = new SimpleDocument(input);
            var output = document.Render(store);

            Assert.That(output, Is.EqualTo(input));
        }

        protected abstract DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration);

        private void AssertRender(string source, DocumentConfiguration configuration, IContext context,
            string expected)
        {
            using (var template = new StringReader(source))
            {
                var result = CreateDocument(template, configuration);
                var output = result.DocumentOrThrow.Render(context);

                Assert.That(output, Is.EqualTo(expected), "Invalid rendered output");
            }
        }

        private void AssertRender(string source, string expected)
        {
            AssertRender(source, default, Context.Empty, expected);
        }

        private void AssertReturn(string source, DocumentConfiguration configuration, IContext context,
            string expected)
        {
            using (var template = new StringReader(source))
            {
                var result = CreateDocument(template, configuration);
                var value = result.DocumentOrThrow.Render(context, new StringWriter());

                Assert.That(value.ToString(), Is.EqualTo(expected), "Invalid return value");
            }
        }

        private void AssertReturn(string source, string expected)
        {
            AssertReturn(source, default, Context.Empty, expected);
        }

        private static IContext CreateContextWithBuiltins(params string[] names)
        {
            var variables = new Dictionary<Value, Value>();

            foreach (var name in names)
            {
                if (!BuiltinFunctions.TryGet(name, out var function))
                    continue;

                variables[name] = new FunctionValue(function);
            }

            return Context.CreateCustom(variables);
        }
    }
}
