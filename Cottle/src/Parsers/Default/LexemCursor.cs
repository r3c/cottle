
namespace Cottle.Parsers.Default
{
	struct LexemCursor
	{
		#region Attributes

		public readonly char Character;

		public LexemState State;

		#endregion

		#region Constructors

		public LexemCursor (char character, LexemState state)
		{
			this.Character = character;
			this.State = state;
		}

		#endregion

		#region Methods

		public void Cancel ()
		{
			this.State = null;
		}

		public LexemCursor Move (char character)
		{
			if (this.State == null)
				return new LexemCursor (this.Character, null);

			return new LexemCursor (this.Character, this.State.Follow (character));
		}

		#endregion
	}
}
