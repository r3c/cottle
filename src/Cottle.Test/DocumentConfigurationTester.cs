using System.Collections.Generic;
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

        public static IEnumerable<TestCaseData> TrimFirstAndLastBlankLinesData_GetData()
        {
            foreach (var eol in new[] { "\n", "\r", "\r\n" })
            {
                foreach (var ws in new[] { " ", "\t", " \t\t " })
                {
                    yield return new TestCaseData($"A{eol}{eol}{ws}", $"A{eol}");
                    yield return new TestCaseData($"A{eol}{ws}B{eol}{ws}", $"A{eol}{ws}B");
                    yield return new TestCaseData($"{eol}{ws}A{eol}{ws}B", $"A{eol}{ws}B");
                    yield return new TestCaseData($"{eol}{ws}A{eol}{ws}B{eol}{ws}", $"A{eol}{ws}B");
                    yield return new TestCaseData($"{eol}{eol}{ws}A", $"{eol}{ws}A");
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(TrimFirstAndLastBlankLinesData_GetData))]
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