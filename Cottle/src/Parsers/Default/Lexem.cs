using System;
using System.Linq;

namespace Cottle.Parsers.Default
{
	struct Lexem
	{
		#region Attributes

		public readonly string Content;

		public readonly LexemType Type;

		#endregion

		#region Constructors

		public Lexem (LexemType type, string content)
		{
			this.Content = content;
			this.Type = type;
		}

		#endregion
	}
}
