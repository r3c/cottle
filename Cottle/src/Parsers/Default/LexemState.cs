using System.Collections.Generic;

namespace Cottle.Parsers.Default
{
	class LexemState
	{
		#region Constants

		private const int BRANCH_LIMIT = 256;

		#endregion

		#region Properties

		public LexemType Type
		{
			get
			{
				return this.type;
			}
		}

		#endregion

		#region Attributes

		private Dictionary<char, LexemState> branchesHigh = null;

		private LexemState[] branchesLow = null;

		private LexemType type = LexemType.None;

		#endregion

		#region Methods

		public LexemState Follow (char character)
		{
			LexemState state;

			if (this.branchesLow != null && character < LexemState.BRANCH_LIMIT)
				return this.branchesLow[character];
			else if (this.branchesHigh != null && this.branchesHigh.TryGetValue (character, out state))
				return state;

			return null;
		}

		public bool Store (LexemType type, string content)
		{
			LexemState current;
			LexemState next;

			current = this;

			foreach (char character in content)
			{
				if (character < LexemState.BRANCH_LIMIT)
				{
					if (current.branchesLow == null)
						current.branchesLow = new LexemState[LexemState.BRANCH_LIMIT];

					next = current.branchesLow[character];

					if (next == null)
					{
						next = new LexemState ();

						current.branchesLow[character] = next;
					}
				}
				else
				{
					if (current.branchesHigh == null)
						current.branchesHigh = new Dictionary<char, LexemState> ();

					if (!current.branchesHigh.TryGetValue (character, out next))
					{
						next = new LexemState ();

						current.branchesHigh[character] = next;
					}
				}

				current = next;
			}

			if (current.type != LexemType.None)
				return false;

			current.type = type;

			return true;
		}

		#endregion
	}
}
