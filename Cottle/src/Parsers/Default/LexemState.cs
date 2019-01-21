using System.Collections.Generic;

namespace Cottle.Parsers.Default
{
    internal class LexemState
    {
        #region Properties

        public LexemType Type { get; private set; } = LexemType.None;

        #endregion

        #region Attributes

        private Dictionary<char, LexemState> _branches;

        #endregion

        #region Methods

        public LexemState Follow(char character)
        {
            if (_branches != null && _branches.TryGetValue(character, out var state))
                return state;

            return null;
        }

        public bool Store(LexemType type, string content)
        {
            var current = this;

            foreach (var character in content)
            {
                if (current._branches == null)
                    current._branches = new Dictionary<char, LexemState>();

                if (!current._branches.TryGetValue(character, out var next))
                {
                    next = new LexemState();

                    current._branches[character] = next;
                }

                current = next;
            }

            if (current.Type != LexemType.None)
                return false;

            current.Type = type;

            return true;
        }

        #endregion
    }
}