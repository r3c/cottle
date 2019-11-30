using Cottle.Builtins;
using NUnit.Framework;

namespace Cottle.Test.Builtins
{
    [TestFixture]
    public class BuiltinTrimmersTester
    {
        [Test]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("  ", " ")]
        [TestCase(" x y z ", " x y z ")]
        [TestCase(" x   y  z ", " x y z ")]
        [TestCase("       x      y     z       ", " x y z ")]
        public void CollapseBlankCharacters(string value, string expected)
        {
            Assert.That(BuiltinTrimmers.CollapseBlankCharacters(value), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("  \n  ", "  \n  ")]
        [TestCase("   \nx y z\n    ", "x y z")]
        [TestCase("   \n \nx y z\n \n    ", " \nx y z\n ")]
        public void FirstAndLastBlankLines(string value, string expected)
        {
            Assert.That(BuiltinTrimmers.FirstAndLastBlankLines(value), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("", "")]
        [TestCase(" ", "")]
        [TestCase("  ", "")]
        [TestCase(" x y z ", "x y z")]
        [TestCase(" x   y  z ", "x   y  z")]
        [TestCase("       x      y     z       ", "x      y     z")]
        public void LeadAndTrailBlankCharacters(string value, string expected)
        {
            Assert.That(BuiltinTrimmers.LeadAndTrailBlankCharacters(value), Is.EqualTo(expected));
        }
    }
}