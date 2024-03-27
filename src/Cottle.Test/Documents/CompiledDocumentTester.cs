using System.Collections.Generic;
using Cottle.Builtins;
using Cottle.Exceptions;
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
        [TestCase(3, "{for i in range(3):}", false)]
        [TestCase(4, "{for i in range(3):}", true)]
        [TestCase(9, "{set i to 0}{while i < 3:{set i to i + 1}}", false)]
        [TestCase(10, "{set i to 0}{while i < 3:{set i to i + 1}}", true)]
        [TestCase(2, "{call()}{call()}{call()}", false)]
        [TestCase(3, "{call()}{call()}{call()}", true)]
        public void Configuration_NbCycleMax(int nbCycleMax, string source, bool expectSuccess)
        {
            var configuration = new DocumentConfiguration { NbCycleMax = nbCycleMax };
            var context = DocumentTester.CreateContextWithBuiltins("range");

            Assert.That(() => AssertOutput(source, configuration, context, string.Empty), expectSuccess
                ? Throws.Nothing
                : Throws.TypeOf<NbCycleExceededException>().With.Property("NbCycleMax").EqualTo(nbCycleMax));
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