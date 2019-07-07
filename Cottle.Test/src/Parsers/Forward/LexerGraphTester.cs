using System.Text;
using Cottle.Parsers.Forward;
using NUnit.Framework;

namespace Cottle.Test.Parsers.Forward
{
    internal class LexerGraphTester
    {
        [TestCase("x", "x", Description = "No match")]
        [TestCase("a", "a", Description = "Incomplete match")]
        [TestCase("ab", "[Bang]", Description = "Full match")]
        [TestCase("xaby", "x[Bang]y", Description = "Inline match")]
        [TestCase("abab", "[Bang][Bang]", Description = "Non-overlapping match sequence")]
        [TestCase("aab", "a[Bang]", Description = "Incomplete match followed by match")]
        [TestCase("bcd", "[Colon]", Description = "Overlapping match")]
        [TestCase("bcdcd", "[Colon][Dot]", Description = "Overlapping match followed by match")]
        [TestCase("abcd", "[Bang][Dot]", Description = "Overlapping match followed by overlapping match")]
        public void MatchPattern(string subject, string expected)
        {
            var graph = new LexerGraph();

            graph.Register("ab", LexemType.Bang);
            graph.Register("bcd", LexemType.Colon);
            graph.Register("cd", LexemType.Dot);
            graph.BuildFallbacks();

            Assert.That(Substitute(graph.Root, subject), Is.EqualTo(expected));
        }

        private string Substitute(LexerNode root, string subject)
        {
            var current = root;
            var output = new StringBuilder();

            foreach (var character in subject)
            {
                current = current.MoveTo(character, output);

                if (current.Type != LexemType.None)
                {
                    output.Append('[').Append(current.Type).Append(']');

                    current = root;
                }
            }

            output.Append(current.FallbackDrop);

            return output.ToString();
        }
    }
}