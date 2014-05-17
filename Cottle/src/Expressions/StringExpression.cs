using System;
using Cottle.Values;

namespace Cottle.Expressions
{
	class StringExpression : ConstantExpression<string>
	{
		#region Constructors

		public	StringExpression (string value) :
			base (new StringValue (value))
		{
		}

		#endregion
	}
}
