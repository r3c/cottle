using System.Collections.Generic;

namespace Cottle.Parsers.Default
{
	class LexemState
	{
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

		private Dictionary<char, LexemState> branches = null;

		private LexemType type = LexemType.None;

		#endregion

		#region Methods

		public LexemState Follow (char character)
		{
			LexemState state;

			if (this.branches != null && this.branches.TryGetValue (character, out state))
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
			    if (current.branches == null)
				    current.branches = new Dictionary<char, LexemState> ();

			    if (!current.branches.TryGetValue (character, out next))
			    {
				    next = new LexemState ();

				    current.branches[character] = next;
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
