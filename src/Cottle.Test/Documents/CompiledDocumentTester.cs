using System;
using System.Collections.Generic;
using System.Threading;
using Cottle.Builtins;
using NUnit.Framework;

namespace Cottle.Test.Documents
{
    public abstract class CompiledDocumentTester : DocumentTester
    {
        protected CompiledDocumentTester(DocumentConfiguration configuration) :
            base(configuration)
        {
        }

        [Test]
        [TestCase("{for i in range(999999999):}")]
        [TestCase("{while 1:}")]
        [TestCase("{set call() to:{wait()}{call()}}{call()}")]
        public void Configuration_Timeout(string source)
        {
            var configuration = new DocumentConfiguration { Timeout = TimeSpan.FromMilliseconds(5) };
            var symbols = new Dictionary<Value, Value>();

            if (BuiltinFunctions.TryGet("range", out var function))
                symbols["range"] = Value.FromFunction(function);

            // Insert some "sleep" function to avoid instantly running into stack overflow exception
            symbols["wait"] = Value.FromFunction(Function.CreatePure0(_ =>
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));

                return Value.Undefined;
            }));

            Assert.That(() => AssertOutput(source, configuration, Context.CreateCustom(symbols), string.Empty),
                Throws.TypeOf<OperationCanceledException>());
        }

        [Test]
        public void Render_StatementDefine()
        {
            var result = AssertOutput("{define var}", string.Empty);

            Assert.That(result.Reports,
                Has.One.Matches<DocumentReport>(r =>
                    r.Severity == DocumentSeverity.Notice && r.Message.Contains("define")));
        }

        [Test]
        [TestCase("declare", "as", "")]
        [TestCase("set", "to", "17")]
        public void Render_StatementSet_FunctionScope(string command, string suffix, string expected)
        {
            AssertOutput("{" + command + " f(x) " + suffix + ":{return x}}" +
                         "{set g(x) to:{return f(x)}}" +
                         "{g(17)}", expected);
        }

        [Test]
        [TestCase("{wrap ucase:{'a'}{unwrap:{'b'}}{'c'}}", "AbC")]
        [TestCase("{wrap ucase:{wrap ord:{'a'}{unwrap:{'b'}}{'c'}}}", "97B99")]
        public void Render_StatementUnwrap_CancelNearestWrap(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ord", "ucase"), expected);
        }

        [Test]
        [TestCase("{wrap ord:{wrap ucase:{'a'}{unwrap:{'b'}{unwrap:{'c'}}{'d'}}{'e'}}}", "6598c10069")]
        public void Render_StatementUnwrap_NestedModifiers(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ord", "ucase"), expected);
        }

        [Test]
        [TestCase("{unwrap:hello}", "hello")]
        [TestCase("{unwrap:{'hello'}}", "hello")]
        public void Render_StatementUnwrap_NothingToCancel(string command, string expected)
        {
            AssertOutput(command, default, Context.Empty, expected);
        }

        [Test]
        [TestCase("{wrap ucase:test}", "test")]
        [TestCase("{wrap ucase:{'test'}}", "TEST")]
        public void Render_StatementWrap_EchoNotLiteral(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ucase"), expected);
        }

        [Test]
        [TestCase("{set f(x) to:{return x + 1}}{wrap f:{1}}", "2")]
        public void Render_StatementWrap_FunctionAsModifier(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins(), expected);
        }

        [Test]
        [TestCase("{wrap ord:{wrap ucase:{'a'}}}", "65")]
        [TestCase("{wrap ucase:{wrap ord:{'a'}}}", "97")]
        [TestCase("{wrap ucase:{'a'}{wrap ord:{'b'}}{'c'}}", "A98C")]
        public void Render_StatementWrap_NestedModifiers(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ord", "ucase"), expected);
        }

        [Test]
        [TestCase("{set f(x) to:{x}}{wrap ucase:{f('a')}}", "A")]
        [TestCase("{wrap ucase:{set f(x) to:{x}}}{f('a')}", "a")]
        public void Render_StatementWrap_ThroughInvokeNotDeclare(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ucase"), expected);
        }
    }
}