
namespace Cottle.Parsers.Default
{
	class LexemCursor
	{
		#region Attributes

		public readonly char	Character;

		public LexemState		State;

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

		public bool Move (char character, out LexemType type)
		{
			if (this.State != null)
			{
				this.State = this.State.Follow (character);

				if (this.State != null && this.State.Type != LexemType.None)
				{
					type = this.State.Type;

					return true;
				}
			}

			type = LexemType.None;

			return false;
		}

		#endregion
	}
}
