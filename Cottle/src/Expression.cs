using System;

namespace Cottle
{
	class Expression
	{
		#region Attributes / Instance

		public Expression[]			Arguments;

		public ExpressionElement[]	Elements;

		public Expression			Source;

		public Expression			Subscript;

		public ExpressionType		Type;

		public Value				Value;

		#endregion

		#region Attributes / Static

		public static readonly Expression	Empty = new Expression
		{
			Type	= ExpressionType.Void
		};

		#endregion
	}
}
