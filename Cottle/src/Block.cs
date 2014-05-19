using System;

namespace Cottle
{
	class Block
	{
		#region Attributes / Instance

		public string[]			Arguments;

		public Block			Body;

		public BlockBranch[]	Branches;

		public string			Key;

		public ScopeMode		Mode;

		public Block			Next;

		public Expression		Source;

		public string			Text;

		public BlockType		Type;

		public string			Value;

		#endregion

		#region Attributes / Static

		public static readonly Block	Empty = new Block
		{
			Type	= BlockType.Void
		};

		#endregion
	}
}
