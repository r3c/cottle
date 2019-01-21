namespace Cottle.Parsers.Default
{
    internal struct LexemCursor
    {
        #region Attributes

        public readonly char Character;

        public LexemState State;

        #endregion

        #region Constructors

        public LexemCursor(char character, LexemState state)
        {
            Character = character;
            State = state;
        }

        #endregion

        #region Methods

        public LexemCursor Move(char character)
        {
            if (State == null)
                return new LexemCursor(Character, null);

            return new LexemCursor(Character, State.Follow(character));
        }

        #endregion
    }
}