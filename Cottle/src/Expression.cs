using System;

namespace Cottle
{
	class Expression
	{
		#region Attributes / Instance

		public Expression[]			Arguments;

		public ExpressionElement[]	Elements;

		public decimal				Number;

		public Expression			Source;

		public string				String;

		public Expression			Subscript;

		public ExpressionType		Type;

		#endregion

		#region Attributes / Static

		public static readonly Expression	Empty = new Expression
		{
			Type	= ExpressionType.Void
		};

		#endregion
	}
}
