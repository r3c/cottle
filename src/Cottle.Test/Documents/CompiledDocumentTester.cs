using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public abstract class CompiledDocumentTester : DocumentTester
    {
        [Test]
        [TestCase("declare", "as", "")]
        [TestCase("set", "to", "17")]
        public void Render_CommandSetFunctionScope(string command, string suffix, string expected)
        {
            AssertOutput("{" + command + " f(x) " + suffix + ":{return x}}" +
                         "{set g(x) to:{return f(x)}}" +
                         "{g(17)}", expected);
        }
    }
}