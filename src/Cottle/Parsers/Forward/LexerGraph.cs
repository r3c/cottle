namespace Cottle.Parsers.Forward
{
    internal class LexerGraph
    {
        public readonly LexerNode Root = new LexerNode();

        public void BuildFallbacks()
        {
            BuildFallbacksForNode(Root, string.Empty);
        }

        public bool Register(string sequence, LexemType type)
        {
            var current = Root;

            foreach (var character in sequence)
                current = current.FollowBy(character);

            if (current.Type != LexemType.None)
                return false;

            current.Type = type;

            return true;
        }

        private void BuildFallbacksForNode(LexerNode current, string prefix)
        {
            if (current.Children is null)
                return;

            foreach (var pair in current.Children)
            {
                var path = prefix + pair.Key;

                // Search for longest suffix of current path having a matching
                // node in our tree, and use this node as a fallback. Since
                // we'll consider empty string as a valid suffix then we'll
                // find a fallback, possibly tree root, for every node.
                for (var i = 1; i <= path.Length; ++i)
                    if (TryFollow(path.Substring(i), out var node))
                    {
                        pair.Value.FallbackDrop = path.Substring(0, i);
                        pair.Value.FallbackNode = node;

                        break;
                    }

                BuildFallbacksForNode(pair.Value, path);
            }
        }

        private bool TryFollow(string sequence, out LexerNode node)
        {
            var current = Root;

            foreach (var character in sequence)
            {
                if (current.Children is null || !current.Children.TryGetValue(character, out current))
                {
                    node = Root;

                    return false;
                }
            }

            node = current;

            return true;
        }
    }
}