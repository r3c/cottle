
namespace Cottle
{
	class LexemCursor
	{
		#region Properties

		public char			Character
		{
			get
			{
				return this.character;
			}
			set
			{
				this.character = value;
			}
		}

		public LexemState	State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		#endregion

		#region Attributes

		private char		character;

		private LexemState	state;

		#endregion

		#region Constructors

		public	LexemCursor (char character, LexemState state)
		{
			this.character = character;
			this.state = state;
		}

		#endregion

		#region Methods

		public void Cancel ()
		{
			this.state = null;
		}

		public bool Move (char character, out LexemType type)
		{
			if (this.state != null)
			{
				this.state = this.state.Follow (character);

				if (this.state != null && this.state.Type != LexemType.None)
				{
					type = this.state.Type;

					return true;
				}
			}

			type = LexemType.None;

			return false;
		}

		#endregion
	}
}
