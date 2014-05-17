using System;
using System.IO;

namespace Cottle.Expressions
{
	abstract class ConstantExpression<T> : IExpression
	{
		#region Attributes

		private readonly Value	value;

		#endregion

		#region Constructors

		protected ConstantExpression (Value value)
		{
			this.value = value;
		}

		#endregion

		#region Methods

		public Value Evaluate (IScope scope, TextWriter output)
		{
			return this.value;
		}

		public override string ToString ()
		{
			return this.value.ToString ();
		}

		#endregion
	}
}
