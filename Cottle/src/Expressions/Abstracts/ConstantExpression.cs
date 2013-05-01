using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace	Cottle.Expressions.Abstracts
{
	abstract class	ConstantExpression<T> : Expression
	{
		#region Attributes

		private Value	value;

		#endregion

		#region Constructors

		protected	ConstantExpression (Value value)
		{
			this.value = value;
		}

		#endregion

		#region Methods

		public override Value	Evaluate (IScope scope, TextWriter output)
		{
			return this.value;
		}

		public override string	ToString ()
		{
			return this.value.ToString ();
		}

		#endregion
	}
}
