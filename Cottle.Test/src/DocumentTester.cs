using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Cottle.Builtins;
using Cottle.Documents;
using Cottle.Functions;
using Cottle.Settings;
using Cottle.Stores;
using Cottle.Values;
using NUnit.Framework;

namespace Cottle.Test
{
    [TestFixture]
    public class DocumentTester
    {
        public static readonly Func<string, ISetting, IDocument>[] Constructors =
        {
            (source, setting) => new DynamicDocument(source, setting),
            (source, setting) => new SimpleDocument(source, setting)
        };

        private void AssertRender(string source, string expected, ISetting setting, Action<IStore> populate,
            Action<IDocument> listen)
        {
            foreach (var constructor in DocumentTester.Constructors)
            {
                var document = constructor(source, setting);

                listen(document);

                IStore store = new SimpleStore();

                populate(store);

                Assert.AreEqual(expected, document.Render(store), "Invalid rendered output for document type '{0}'",
                    document.GetType());
            }
        }

        private void AssertRender(string source, string expected)
        {
            AssertRender(source, expected, DefaultSetting.Instance, s => { }, d => { });
        }

        private void AssertReturn(string source, string expected, ISetting setting, Action<IStore> populate,
            Action<IDocument> listen)
        {
            foreach (var constructor in DocumentTester.Constructors)
            {
                var document = constructor(source, setting);

                listen(document);

                IStore store = new SimpleStore();

                populate(store);

                var value = document.Render(store, new StringWriter());

                Assert.AreEqual(expected, value.ToString(), "Invalid return value for document type '{0}'",
                    document.GetType());
            }
        }

        private void AssertReturn(string source, string expected)
        {
            AssertReturn(source, expected, DefaultSetting.Instance, s => { }, d => { });
        }

        private Action<IStore> PopulateScope(params string[] names)
        {
            return s =>
            {
                foreach (var name in names)
                {
                    if (!BuiltinFunctions.TryGet(name, out var function))
                        continue;

                    s[name] = new FunctionValue(function);
                }
            };
        }

        [Test]
        public void CommandDeclare()
        {
            AssertRender("{declare var}", string.Empty);
        }

        [Test]
        [TestCase("expression", "{3}", "3")]
        [TestCase("mixed", "A{'B'}C", "ABC")]
        [TestCase("number", "1", "1")]
        [TestCase("text", "something", "something")]
        public void CommandDeclareRender(string name, string body, string expected)
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
        public void CommandDeclareValueSimple(string name, string value, string expected)
        {
            AssertReturn("{declare " + name + " as " + value + "}{return " + name + "}", expected);
        }

        [Test]
        [TestCase("5", "5")]
        [TestCase("\"Hello, World!\"", "Hello, World!")]
        public void CommandEcho(string value, string expected)
        {
            AssertRender("{echo " + value + "}", expected);
        }

        [Test]
        [TestCase("k", "v", "[]", "-", "EMPTY")]
        [TestCase("key", "value", "['A': 'X', 'B': 'Y', 'C': 'Z']", "{key}{value}", "AXBYCZ")]
        [TestCase("i", "j", "[1, 5, 9]", "{i}{j}", "011529")]
        public void CommandForKeyValue(string name1, string name2, string source, string body, string expected)
        {
            AssertRender("{for " + name1 + ", " + name2 + " in " + source + ":" + body + "|empty:EMPTY}", expected);
        }

        [Test]
        [TestCase("unused", "[]", "-", "EMPTY")]
        [TestCase("dummy", "[1, 5, 9]", "X", "XXX")]
        [TestCase("v", "[1, 5, 9]", "{v}", "159")]
        [TestCase("name", "[5: 'A', 9: 'B', 2: 'C']", "{name}", "ABC")]
        public void CommandForValue(string name, string source, string body, string expected)
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
        public void CommandIf(string condition, string body, string expected)
        {
            AssertRender("{if " + condition + ":" + body + "}", expected);
        }

        [Test]
        [TestCase("1", "1")]
        [TestCase("'A'", "\"A\"")]
        [TestCase("[]", "[]")]
        public void CommandReturnDirect(string value, string expected)
        {
            AssertReturn("{return " + value + "}", expected);
        }

        [Test]
        [TestCase("0", "0", "\"B\"")]
        [TestCase("0", "1", "\"B\"")]
        [TestCase("1", "0", "\"AB\"")]
        [TestCase("1", "1", "\"AA\"")]
        public void CommandReturnNested(string a, string b, string expected)
        {
            AssertReturn("{if " + a + ":{if " + b + ":{return 'AA'}|else:{return 'AB'}}|else:{return 'B'}}", expected);
        }

        [Test]
        public void CommandSet()
        {
            AssertRender("{set var}", string.Empty);
        }

        [Test]
        [TestCase("{set a to 1}{set b to 2}", "", "{a}{b}", "12")]
        [TestCase("{set a to 1}{set b to 2}", "{a}{b}", "{a}{b}", "3412")]
        public void CommandSetFunctionArguments(string pre, string body, string post, string expected)
        {
            AssertRender(pre + "{set f(a, b) to:" + body + "}{f(3, 4)}" + post, expected);
        }

        [Test]
        [TestCase("f", "Hello, World!", "Hello, World!")]
        [TestCase("test", "{'Some String'}", "Some String")]
        public void CommandSetFunctionRender(string name, string body, string expected)
        {
            AssertRender("{set " + name + "() to:" + body + "}{return " + name + "()}", expected);
        }

        [Test]
        [TestCase("f", "{return 1}", "1")]
        [TestCase("test", "{return 'Some String'}", "\"Some String\"")]
        [TestCase("x", "{if 1:{return 'X'}|else:{return 'Y'}}", "\"X\"")]
        public void CommandSetFunctionReturn(string name, string body, string expected)
        {
            AssertReturn("{set " + name + "(a) to:" + body + "}{return " + name + "()}", expected);
        }

        [Test]
        [TestCase("var", "1", "1")]
        [TestCase("_", "'A'", "\"A\"")]
        [TestCase("some_symbol_name", "[]", "[]")]
        public void CommandSetValueScope(string name, string value, string expected)
        {
            AssertReturn(
                "{declare f() as:{set " + name + " to " + value + "}}{set " + name + " to 'default'}{f()}{return " +
                name + "}", expected);
        }

        [Test]
        [TestCase("var", "1", "1")]
        [TestCase("_", "'A'", "\"A\"")]
        [TestCase("some_symbol_name", "[]", "[]")]
        public void CommandSetValueSimple(string name, string value, string expected)
        {
            AssertReturn("{set " + name + " to " + value + "}{return " + name + "}", expected);
        }

        [Test]
        [TestCase("{set a to 0}", "lt(a, 8)", "{set a to add(a, 1)}{a}", "12345678")]
        [TestCase("{set a to 8}", "lt(0, a)", "{set a to add(a, -1)}X", "XXXXXXXX")]
        public void CommandWhile(string init, string condition, string body, string expected)
        {
            AssertRender(init + "{while " + condition + ":" + body + "}", expected, DefaultSetting.Instance,
                PopulateScope("add", "lt"), d => { });
        }

        [Test]
        [TestCase("{\"\\\"\"}", '\\', "\"")]
        [TestCase("{\"xxxy\"}", 'x', "xy")]
        public void EscapeCommand(string input, char escape, string expected)
        {
            var setting = new CustomSetting { Escape = escape };

            AssertRender(input, expected, setting, s => { }, d => { });
        }

        [Test]
        [TestCase("Escaped\\ Literal", '\\', "Escaped Literal")]
        [TestCase("-{'x'-}", '-', "{'x'}")]
        [TestCase("ABC", 'x', "ABC")]
        [TestCase("--", '-', "-")]
        public void EscapeText(string input, char escape, string expected)
        {
            var setting = new CustomSetting { Escape = escape };

            AssertRender(input, expected, setting, s => { }, d => { });
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
        public void ExpressionAccess(string access, string expected)
        {
            void Populate(IStore scope)
            {
                scope["aaa"] = new[] { 5, (Value)7 };
                scope["bbb"] = new Dictionary<Value, Value> { { "x", "$X$" }, { "y", "$Y$" } };
                scope["ccc"] = new Dictionary<Value, Value>
                    { { "A", new Dictionary<Value, Value> { { "i", 50 } } }, { 1, 42 } };
            }

            AssertReturn("{return " + access + "}", expected, DefaultSetting.Instance, Populate, d => { });
        }

        [Test]
        [TestCase("42", "42")]
        [TestCase("-17.2", "-17.2")]
        [TestCase("\"42\"", "\"42\"")]
        [TestCase("\"ABC\"", "\"ABC\"")]
        public void ExpressionConstant(string constant, string expected)
        {
            AssertReturn("{return " + constant + "}", expected);
        }

        [Test]
        [TestCase("abc", "42")]
        [TestCase("xyz", "17")]
        public void ExpressionInvoke(string symbol, string expected)
        {
            void Populate(IStore scope)
            {
                scope[symbol] = expected;
                scope["f"] = new NativeFunction((a, s, o) =>
                {
                    var value = s[a[0]].AsString;

                    o.Write(value);

                    return value;
                }, 1);
            }

            AssertRender("{f('" + symbol + "')}", ((Value)expected).AsString + ((Value)expected).AsString,
                DefaultSetting.Instance, Populate, d => { });
            AssertReturn("{return f('" + symbol + "')}", expected, DefaultSetting.Instance,
                Populate, d => { });
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
        public void ExpressionMap(string expression, string expected)
        {
            AssertReturn("{return " + expression + "}", expected);
        }

        [Test]
        [TestCase("aaa", "aaa", "I sense a soul")]
        [TestCase("_", "_", "in search of answers")]
        [TestCase("x", "missing", "x")]
        public void ExpressionSymbol(string set, string get, string value)
        {
            var expected = (set == get ? (Value)value : VoidValue.Instance).ToString();

            void Populate(IStore scope)
            {
                scope[set] = value;
            }

            AssertReturn("{return " + get + "}", expected, DefaultSetting.Instance, Populate, d => { });
        }

        [Test]
        public void OptimizeConstantMap()
        {
            var setting = new CustomSetting { Optimize = false };

            AssertRender("{['X', 'Y', 'Z'][0]}", "X", setting, s => { }, d => { });

            setting.Optimize = true;

            AssertRender("{['X', 'Y', 'Z'][0]}", "X", setting, s => { }, d => { });
        }

        [Test]
        public void OptimizeReturn()
        {
            var setting = new CustomSetting { Optimize = false };

            AssertRender("X{return 1}Y", "X", setting, s => { }, d => { });

            setting.Optimize = true;

            AssertRender("X{return 1}Y", "X", setting, s => { }, d => { });
        }

        [Test]
        [TestCase("1", "1")]
        [TestCase("3", "6")]
        [TestCase("8", "40320")]
        public void SampleFactorial(string value, string expected)
        {
            AssertReturn
            (
                "{set factorial(n) to:" +
                "{if gt(n, 1):" +
                "{return mul(n, factorial(sub(n, 1)))}" +
                "|else:" +
                "{return 1}" +
                "}" +
                "}" +
                "{return factorial(" + value + ")}",
                expected,
                DefaultSetting.Instance,
                PopulateScope("gt", "mul", "sub"),
                d => { }
            );
        }

        [Test]
        [TestCase("2", "A -> B, A -> C, B -> C, ")]
        [TestCase("3", "A -> C, A -> B, C -> B, A -> C, B -> A, B -> C, A -> C, ")]
        public void SampleHanoiTowers(string disks, string expected)
        {
            AssertRender
            (
                "{set hanoi_rec(n, from, by, to) to:" +
                "{set n to sub(n, 1)}" +
                "{if gt(n, 0):" +
                "{hanoi_rec(n, from, to, by)}" +
                "}" +
                "{from} -> {to}, " +
                "{if gt(n, 0):" +
                "{hanoi_rec(n, by, from, to)}" +
                "}" +
                "}" +
                "{set hanoi(n) to:" +
                "{hanoi_rec(n, 'A', 'B', 'C')}" +
                "}" +
                "{hanoi(" + disks + ")}",
                expected,
                DefaultSetting.Instance,
                PopulateScope("gt", "sub"),
                d => { }
            );
        }

        [Test]
        [TestCase("2", "4")]
        [TestCase("8", "256")]
        [TestCase("16", "65536")]
        public void SamplePowerOfTwo(string value, string expected)
        {
            AssertReturn
            (
                "{set a to " + value + "}" +
                "{set b to 1}" +
                "{while gt (a, 0):" +
                "{set a to add (a, -1)}" +
                "{set b to add (b, b)}" +
                "}" +
                "{return b}",
                expected,
                DefaultSetting.Instance,
                PopulateScope("add", "gt"),
                d => { }
            );
        }

        [Test]
        [TestCase("<|", "|>", "1 || 0", "true")]
        [TestCase("<&", "&>", "1 && 0", "")]
        public void TestAmbiguous(string blockBegin, string blockEnd, string expression, string expected)
        {
            var setting = new CustomSetting { BlockBegin = blockBegin, BlockContinue = "<>", BlockEnd = blockEnd };

            AssertRender(blockBegin + expression + blockEnd, expected, setting, s => { }, d => { });
        }

        [Test]
        public void TextEmpty()
        {
            AssertRender(string.Empty, string.Empty);
        }

        [Test]
        [TestCase("\\\\", "\\")]
        [TestCase("\\{\\|\\}", "{|}")]
        [TestCase("a\\{b\\|c\\}d", "a{b|c}d")]
        public void TextEscape(string escaped, string expected)
        {
            AssertRender(escaped, expected);
        }

        [Test]
        [TestCase("Hello, World!")]
        [TestCase("This is some literal text")]
        public void TextLiteral(string expected)
        {
            AssertRender(expected, expected);
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

            Assert.AreEqual(input, output);
        }

        [Test]
        [TestCase("A", "A", "B", "B")]
        [TestCase("X  Y", " +", " ", "X Y")]
        [TestCase("df98gd76dfg5df4g321gh0", "[^0-9]", "", "9876543210")]
        public void TextTrim(string value, string pattern, string replacement, string expected)
        {
            var setting = new CustomSetting { Trimmer = s => Regex.Replace(s, pattern, replacement) };

            AssertRender(value, expected, setting, s => { }, d => { });
        }
    }
}