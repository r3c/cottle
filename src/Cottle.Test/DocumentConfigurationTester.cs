using NUnit.Framework;

namespace Cottle.Test
{
    public class DocumentConfigurationTester
    {
        [Test]
        [TestCase("", "")]
        [TestCase(" ", "")]
        [TestCase("  ", "")]
        [TestCase(" x y z ", "x y z")]
        [TestCase(" x \n y \r z ", "x \n y \r z")]
        [TestCase("       x   \r\n   y  \n\r  z       ", "x   \r\n   y  \n\r  z")]
        public void TrimEnclosingWhitespaces(string input, string expected)
        {
            Assert.That(DocumentConfiguration.TrimEnclosingWhitespaces(input), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("\n  A\n  B", "A\n  B")]
        [TestCase("A\n  B\n  ", "A\n  B")]
        [TestCase("\n  A\n  B\n  ", "A\n  B")]
        [TestCase("\r\n\t A\r\n  \n\rB\n\r\t ", "A\r\n  \n\rB")]
        public void TrimFirstAndLastBlankLines(string input, string expected)
        {
            Assert.That(DocumentConfiguration.TrimFirstAndLastBlankLines(input), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("A", "A")]
        [TestCase(" A", " A")]
        [TestCase("A ", "A ")]
        public void TrimNothing(string input, string expected)
        {
            Assert.That(DocumentConfiguration.TrimNothing(input), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("  ", " ")]
        [TestCase(" \t ", " ")]
        [TestCase("\t \t", " ")]
        [TestCase(" x y z ", " x y z ")]
        [TestCase(" x   y  z ", " x y z ")]
        [TestCase("       x      y     z       ", " x y z ")]
        public void TrimRepeatedWhitespaces(string input, string expected)
        {
            Assert.That(DocumentConfiguration.TrimRepeatedWhitespaces(input), Is.EqualTo(expected));
        }
    }
}