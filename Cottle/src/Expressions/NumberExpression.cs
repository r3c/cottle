using System;

using Cottle.Expressions.Abstracts;
using Cottle.Values;

namespace Cottle.Expressions
{
	sealed class NumberExpression : ConstantExpression<decimal>
	{
		#region Constructors

		public	NumberExpression (decimal value) :
			base (new NumberValue (value))
		{
		}

		#endregion
	}
}
