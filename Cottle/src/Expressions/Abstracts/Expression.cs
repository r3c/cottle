using System;
using System.IO;

namespace Cottle.Expressions.Abstracts
{
	abstract class Expression : IExpression
	{
		#region Methods

		public abstract Value			Evaluate (IScope scope, TextWriter output);

		public abstract override string ToString ();

		#endregion
	}
}
