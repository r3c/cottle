using System.Collections.Generic;

namespace	Cottle
{
	class	LexemState
	{
		#region Properties

		public LexemType	Type
		{
			get
			{
				return this.type;
			}
		}

		#endregion

		#region Attributes

		private Dictionary<char, LexemState>	branches = new Dictionary<char, LexemState> ();

		private LexemType						type = LexemType.None;

		#endregion

		#region Methods

		public LexemState	Follow (char character)
		{
			LexemState	state;

			if (this.branches.TryGetValue (character, out state))
				return state;

			return null;
		}

		public bool Store (LexemType type, string content)
		{
			char		character;
			LexemState	state;

			if (content.Length > 0)
			{
				character = content[0];

				if (!this.branches.TryGetValue (character, out state))
				{
					state = new LexemState ();

					this.branches[character] = state;
				}

				return state.Store (type, content.Substring (1));
			}
			else if (this.type == LexemType.None)
			{
				this.type = type;

				return true;
			}

			return false;
		}

		#endregion
	}
}
