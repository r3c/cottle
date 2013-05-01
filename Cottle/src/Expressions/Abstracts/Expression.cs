using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace	Cottle.Expressions.Abstracts
{
	abstract class	Expression : IExpression
	{
		#region Methods

		public abstract Value			Evaluate (IScope scope, TextWriter output);

		public abstract override string ToString ();

		#endregion
	}
}
