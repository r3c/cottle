using System.IO;

using Cottle.Expressions.Abstracts;
using Cottle.Values;

namespace	Cottle.Expressions
{
	sealed class	UndefinedExpression : Expression
	{
		#region Attributes

		public static readonly UndefinedExpression	Instance = new UndefinedExpression ();

		#endregion

		#region Methods

		public override Value	Evaluate (IScope scope, TextWriter output)
		{
			return UndefinedValue.Instance;
		}

		public override string	ToString ()
		{
			return UndefinedValue.Instance.ToString ();
		}

		#endregion
	}
}
