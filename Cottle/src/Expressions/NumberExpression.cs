using System;
using Cottle.Values;

namespace Cottle.Expressions
{
	class NumberExpression : ConstantExpression<decimal>
	{
		#region Constructors

		public	NumberExpression (decimal value) :
			base (new NumberValue (value))
		{
		}

		#endregion
	}
}
