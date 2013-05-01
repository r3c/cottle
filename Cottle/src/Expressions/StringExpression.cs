using System;
using System.Collections.Generic;
using System.Text;

using Cottle.Expressions.Abstracts;
using Cottle.Values;

namespace	Cottle.Expressions
{
	sealed class	StringExpression : ConstantExpression<string>
	{
		#region Constructors

		public	StringExpression (string value) :
			base (new StringValue (value))
		{
		}

		#endregion
	}
}
