using NUnit.Framework;

namespace Cottle.Test.Documents
{
    [TestFixture]
    public abstract class CompiledDocumentTester : IDocumentTester
    {
        [Test]
        [TestCase("declare", "as", "")]
        [TestCase("set", "to", "17")]
        public void RenderCommandSetFunctionScope(string command, string suffix, string expected)
        {
            AssertRender("{" + command + " f(x) " + suffix + ":{return x}}" +
                         "{set g(x) to:{return f(x)}}" +
                         "{g(17)}", expected);
        }
    }
}