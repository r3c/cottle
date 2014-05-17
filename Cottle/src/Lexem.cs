using System;
using System.Linq;

namespace Cottle
{
	struct	Lexem
	{
		#region Properties

		public string		Content
		{
			get
			{
				return this.content;
			}
		}

		public LexemType	Type
		{
			get
			{
				return this.type;
			}
		}

		#endregion

		#region Attributes

		private readonly string		content;

		private readonly LexemType	type;

		#endregion

		#region Constructors

		public	Lexem (LexemType type, string content)
		{
			this.content = content;
			this.type = type;
		}

		#endregion
	}
}
