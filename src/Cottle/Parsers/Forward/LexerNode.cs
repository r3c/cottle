using System.Collections.Generic;
using System.Text;

namespace Cottle.Parsers.Forward
{
    internal class LexerNode
    {
        public Dictionary<char, LexerNode>? Children;

        public string? FallbackDrop;

        public LexerNode? FallbackNode;

        public LexemType Type;

        public LexerNode FollowBy(char character)
        {
            if (Children is null)
                Children = new Dictionary<char, LexerNode>();

            if (Children.TryGetValue(character, out var child))
                return child;

            child = new LexerNode();

            Children[character] = child;

            return child;
        }

        public LexerNode MoveTo(char character, StringBuilder output)
        {
            for (var current = this; ; current = current.FallbackNode)
            {
                if (current.Children is not null && current.Children.TryGetValue(character, out var next))
                    return next;

                if (current.FallbackNode is null)
                {
                    output.Append(character);

                    return current;
                }

                output.Append(current.FallbackDrop);
            }
        }
    }
}