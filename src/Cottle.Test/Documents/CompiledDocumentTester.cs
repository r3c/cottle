using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public abstract class CompiledDocumentTester : DocumentTester
    {
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
        [TestCase("{wrap ord:{wrap ucase:{'a'}{unwrap:{'b'}{unwrap:{'c'}}{'d'}}{'e'}}}", "6598c10069")]
        public void Render_StatementUnwrap_Nested(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ord", "ucase"), expected);
        }

        [Test]
        [TestCase("{unwrap:hello}", "hello")]
        [TestCase("{unwrap:{'hello'}}", "hello")]
        public void Render_StatementUnwrap_Nothing(string command, string expected)
        {
            AssertOutput(command, default, Context.Empty, expected);
        }

        [Test]
        [TestCase("{wrap ucase:{'a'}{unwrap:{'b'}}{'c'}}", "AbC")]
        [TestCase("{wrap ucase:{wrap ord:{'a'}{unwrap:{'b'}}{'c'}}}", "97B99")]
        public void Render_StatementUnwrap_Single(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ord", "ucase"), expected);
        }

        [Test]
        [TestCase("{set f(x) to:{x}}{wrap ucase:{f('a')}}", "A")]
        [TestCase("{wrap ucase:{set f(x) to:{x}}}{f('a')}", "a")]
        public void Render_StatementWrap_Function(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ucase"), expected);
        }

        [Test]
        [TestCase("{wrap ord:{wrap ucase:{'a'}}}", "65")]
        [TestCase("{wrap ucase:{wrap ord:{'a'}}}", "97")]
        [TestCase("{wrap ucase:{'a'}{wrap ord:{'b'}}{'c'}}", "A98C")]
        public void Render_StatementWrap_Nested(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ord", "ucase"), expected);
        }

        [Test]
        [TestCase("{wrap ucase:test}", "test")]
        [TestCase("{wrap ucase:{'test'}}", "TEST")]
        public void Render_StatementWrap_Single(string command, string expected)
        {
            AssertOutput(command, default, DocumentTester.CreateContextWithBuiltins("ucase"), expected);
        }
    }
}