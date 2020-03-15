using System.Collections.Generic;
using System.IO;
using System.Text;
using Cottle.Builtins;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test
{
    [TestFixture]
    public abstract class DocumentTester
    {
        [Test]
        [TestCase("A{x}B", "AB")]
        [TestCase("A{x}B{y}C", "ABC")]
        public void Render_CommandComposite(string expression, string expected)
        {
            AssertOutput(expression, expected);
        }

        [Test]
        public void Render_CommandDeclare()
        {
            AssertOutput("{declare var}", string.Empty);
        }

        [Test]
        [TestCase("expression", "{3}", "3")]
        [TestCase("mixed", "A{'B'}C", "ABC")]
        [TestCase("number", "1", "1")]
        [TestCase("text", "something", "something")]
        public void Render_CommandDeclareRender(string name, string body, string expected)
        {
            AssertOutput("{declare " + name + " as:" + body + "}{echo " + name + "}", expected);
        }

        [Test]
        [TestCase("declare", "as")]
        [TestCase("set", "to")]
        public void Render_CommandDeclareValueScope(string command, string suffix)
        {
            AssertReturn("{" + command + " f() " + suffix + ":{declare x as 'unused'}}" +
                         "{" + command + " x " + suffix + " 42}" +
                         "{f()}" +
                         "{return x}", "42");
        }

        [Test]
        [TestCase("var", "1", "1")]
        [TestCase("_", "'A'", "\"A\"")]
        [TestCase("some_symbol_name", "[]", "[]")]
        public void Render_CommandDeclareValueSimple(string name, string value, string expected)
        {
            AssertReturn("{declare " + name + " as " + value + "}{return " + name + "}", expected);
        }

        [Test]
        [TestCase("5", "5")]
        [TestCase("\"Hello, World!\"", "\"Hello, World!\"")]
        [TestCase("[1, 2]", "[1, 2]")]
        [TestCase("[0: 1, 1: 2]", "[1, 2]")]
        [TestCase("[0: 1, 2, 2: 3]", "[1, 2, 3]")]
        [TestCase("[0: 1, 2, 3: 3]", "[1, 2, 3: 3]")]
        [TestCase("[1: 1, 2, 1: 3]", "[1: 1, 2, 3]")]
        public void Render_CommandDump(string value, string expected)
        {
            AssertOutput("{dump " + value + "}", expected);
        }

        [Test]
        [TestCase("5", "5")]
        [TestCase("\"Hello, World!\"", "Hello, World!")]
        [TestCase("[1, 2]", "")]
        public void Render_CommandEcho(string value, string expected)
        {
            AssertOutput("{echo " + value + "}", expected);
        }

        [Test]
        [TestCase("_", "[]", "something", "array", "array")]
        [TestCase("_", "17", "something", "scalar", "scalar")]
        [TestCase("_", "\"\"", "something", "", "")]
        [TestCase("dummy", "[1, 5, 9]", "X", null, "XXX")]
        [TestCase("v", "[1, 5, 9]", "{v}", null, "159")]
        [TestCase("name", "[5: 'A', 9: 'B', 2: 'C']", "{name}", null, "ABC")]
        [TestCase("k, v", "[]", "-", "array", "array")]
        [TestCase("key, value", "['A': 'X', 'B': 'Y', 'C': 'Z']", "{key}{value}", null, "AXBYCZ")]
        [TestCase("i, j", "[1, 5, 9]", "{i}{j}", null, "011529")]
        public void Render_CommandFor(string pairs, string source, string body, string empty, string expected)
        {
            AssertOutput("{for " + pairs + " in " + source + ":" + body + (empty != null ? "|empty:" + empty : string.Empty) + "}", expected);
        }

        [Test]
        [TestCase("1", "true", "true")]
        [TestCase("''", "invisible", "")]
        [TestCase("'something'", "visible", "visible")]
        [TestCase("[]", "invisible", "")]
        [TestCase("[1, 2, 3]", "visible", "visible")]
        [TestCase("1", "a|else:b", "a")]
        [TestCase("0", "a|else:b", "b")]
        [TestCase("1", "a|elif 1:b|else:c", "a")]
        [TestCase("0", "a|elif 1:b|else:c", "b")]
        [TestCase("0", "a|elif 0:b|else:c", "c")]
        [TestCase("1 + 0", "a|elif 1 + 0:b|else:c", "a")]
        [TestCase("0 + 0", "a|elif 1 + 0:b|else:c", "b")]
        [TestCase("0 + 0", "a|elif 0 + 0:b|else:c", "c")]
        public void Render_CommandIf(string condition, string body, string expected)
        {
            AssertOutput("{if " + condition + ":" + body + "}", expected);
        }

        [Test]
        [TestCase("1", "1")]
        [TestCase("'A'", "\"A\"")]
        [TestCase("[]", "[]")]
        public void Render_CommandReturnDirect(string value, string expected)
        {
            AssertReturn("{return " + value + "}", expected);
        }

        [Test]
        [TestCase("0", "0", "\"B\"")]
        [TestCase("0", "1", "\"B\"")]
        [TestCase("1", "0", "\"AB\"")]
        [TestCase("1", "1", "\"AA\"")]
        public void Render_CommandReturnNested(string a, string b, string expected)
        {
            AssertReturn("{if " + a + ":{if " + b + ":{return 'AA'}|else:{return 'AB'}}|else:{return 'B'}}", expected);
        }

        [Test]
        public void Render_CommandSet()
        {
            AssertOutput("{set var}", string.Empty);
        }

        [Test]
        [TestCase("{set a to 1}{set b to 2}", "", "{a}{b}", "12")]
        [TestCase("{set a to 1}{set b to 2}", "{a}{b}", "{a}{b}", "3412")]
        public void Render_CommandSetFunctionArguments(string pre, string body, string post, string expected)
        {
            AssertOutput(pre + "{set f(a, b) to:" + body + "}{f(3, 4)}" + post, expected);
        }

        [Test]
        [TestCase("f", "Hello, World!", "Hello, World!")]
        [TestCase("test", "{'Some String'}", "Some String")]
        [TestCase("test", "AAA{return 0}BBB", "AAA")]
        public void Render_CommandSetFunctionRender(string name, string body, string expected)
        {
            AssertOutput("{set " + name + "() to:" + body + "}{return " + name + "()}", expected);
        }

        [Test]
        [TestCase("f", "{return 1}", "1")]
        [TestCase("test", "{return 'Some String'}", "\"Some String\"")]
        [TestCase("x", "{if 1:{return 'X'}|else:{return 'Y'}}", "\"X\"")]
        public void Render_CommandSetFunctionReturn(string name, string body, string expected)
        {
            AssertReturn("{set " + name + "(a) to:" + body + "}{return " + name + "()}", expected);
        }

        [Test]
        [TestCase("x", "123", "123")]
        [TestCase("y", "{'a'}b{'c'}", "abc")]
        [TestCase("z", "A{return void}B", "A")]
        public void Render_CommandSetRender(string name, string body, string expected)
        {
            AssertOutput("{set f() to:{set " + name + " to:" + body + "}}{f()}{echo " + name + "}", expected);
        }

        [Test]
        [TestCase("{while parent:{parent.value}{set parent to parent.child}}")]
        public void Render_CommandSetValueInLoop(string source)
        {
            AssertOutput(source, default, Context.CreateCustom(
                new Dictionary<Value, Value>
                {
                    ["parent"] = new Dictionary<Value, Value>
                        { ["child"] = new Dictionary<Value, Value> { ["value"] = "53" }, ["value"] = "42" }
                }), "4253");
        }

        [Test]
        [TestCase("define", "as", "default")]
        [TestCase("set", "to", "override")]
        public void Render_CommandSetValueScope(string command, string suffix, string expected)
        {
            AssertOutput("{declare f() as:{" + command + " x " + suffix + " 'override'}}" +
                         "{set x to 'default'}" +
                         "{f()}" +
                         "{x}", expected);
        }

        [Test]
        [TestCase("var", "1", "1")]
        [TestCase("_", "'A'", "\"A\"")]
        [TestCase("some_symbol_name", "[]", "[]")]
        public void Render_CommandSetValueSimple(string name, string value, string expected)
        {
            AssertReturn("{set " + name + " to " + value + "}{return " + name + "}", expected);
        }

        [Test]
        [TestCase("{set a to 0}", "lt(a, 8)", "{set a to add(a, 1)}{a}", "12345678")]
        [TestCase("{set a to 8}", "lt(0, a)", "{set a to add(a, -1)}X", "XXXXXXXX")]
        public void Render_CommandWhile(string init, string condition, string body, string expected)
        {
            AssertOutput(init + "{while " + condition + ":" + body + "}", default,
                DocumentTester.CreateContextWithBuiltins("add", "lt"), expected);
        }

        [Test]
        [TestCase("Hello, World!")]
        [TestCase("This is some literal text")]
        public void Render_Literal(string expected)
        {
            AssertOutput(expected, expected);
        }

        [Test]
        public void Render_LiteralEmpty()
        {
            AssertOutput(string.Empty, string.Empty);
        }

        [Test]
        [TestCase("\\\\", "\\")]
        [TestCase("\\{\\|\\}", "{|}")]
        [TestCase("a\\{b\\|c\\}d", "a{b|c}d")]
        public void Render_LiteralEscape(string escaped, string expected)
        {
            AssertOutput(escaped, expected);
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
        public void Render_ExpressionAccess(string access, string expected)
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
        public void Render_ExpressionConstant(string constant, string expected)
        {
            AssertReturn("{return " + constant + "}", expected);
        }

        [Test]
        [TestCase("\"abc\"", "abcabc")]
        [TestCase("17", "1717")]
        public void Render_ExpressionInvoke(string input, string expected)
        {
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                ["f"] = new FunctionValue(Function.CreatePure1((state, value) => value.AsString + value.AsString))
            });

            AssertOutput($"{{f({input})}}", default, context, expected);
        }

        [Test]
        [TestCase("[][0]", "<void>")]
        [TestCase("[0][0]", "0")]
        [TestCase("[5][0]", "5")]
        [TestCase("[5][1]", "<void>")]
        [TestCase("[5, 8, 2][1]", "8")]
        [TestCase("['implicit', 'second'][0]", "\"implicit\"")]
        [TestCase("['implicit', 1: 'explicit'][0]", "\"implicit\"")]
        [TestCase("['implicit', 0: 'explicit'][0]", "\"explicit\"")]
        [TestCase("[5, 2, 'A', 2][2]", "\"A\"")]
        [TestCase("[2: 'A', 5: 'B'][0]", "<void>")]
        [TestCase("[2: 'A', 5: 'B'][2]", "\"A\"")]
        [TestCase("[2: 'A', 5: 'B']['2']", "<void>")]
        [TestCase("['x': 'X', 'y': 'Y']['y']", "\"Y\"")]
        [TestCase("['a': ['b': ['c': 42]]]['a']['b']['c']", "42")]
        public void Render_ExpressionMap(string expression, string expected)
        {
            AssertReturn("{return " + expression + "}", expected);
        }

        [Test]
        [TestCase("aaa", "aaa", "I sense a soul", "\"I sense a soul\"")]
        [TestCase("_", "_", "in search of answers", "\"in search of answers\"")]
        [TestCase("x", "missing", "x", "<void>")]
        public void Render_ExpressionSymbol(string set, string get, string value, string expected)
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
        public void Render_SampleFactorial(string value, string expected)
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
        public void Render_SampleHanoiTowers(string disks, string expected)
        {
            AssertOutput(@"
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
        public void Render_SamplePowerOfTwo(string value, string expected)
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
        [TestCase(false, 45000)]
        [TestCase(false, 90000)]
        [TestCase(false, 200000)]
        [TestCase(true, 45000)]
        [TestCase(true, 90000)]
        [TestCase(true, 200000)]
        public void Render_TextTooLong(bool variable, int characters)
        {
            var builder = new StringBuilder();

            // Add some variables in the text to test a use case where we don't have only text
            if (variable)
                builder.Append("{foo}");

            builder.Append('a', characters);

            // Add some variables in the text to test a use case where we don't have only text
            if (variable)
                builder.Append("{foo}");

            // Variable "foo" renders to itself
            var context = Context.CreateCustom(new Dictionary<Value, Value>
            {
                { "foo", "{foo}" }
            });

            var input = builder.ToString();

            AssertOutput(input, default, context, input);
        }

        protected abstract DocumentResult CreateDocument(TextReader template, DocumentConfiguration configuration);

        private void AssertOutput(string source, DocumentConfiguration configuration, IContext context,
            string expected)
        {
            using (var template = new StringReader(source))
            {
                var result = CreateDocument(template, configuration);
                var output = result.DocumentOrThrow.Render(context);

                Assert.That(output, Is.EqualTo(expected), "Invalid rendered output");
            }
        }

        protected void AssertOutput(string source, string expected)
        {
            AssertOutput(source, default, Context.Empty, expected);
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
            var symbols = new Dictionary<Value, Value>();

            foreach (var name in names)
            {
                if (!BuiltinFunctions.TryGet(name, out var function))
                    continue;

                symbols[name] = new FunctionValue(function);
            }

            return Context.CreateCustom(symbols);
        }
    }
}
