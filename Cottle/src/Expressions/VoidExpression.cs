using System.IO;

using Cottle.Expressions.Abstracts;
using Cottle.Values;

namespace Cottle.Expressions
{
	sealed class VoidExpression : Expression
	{
		#region Attributes

		public static readonly VoidExpression	Instance = new VoidExpression ();

		#endregion

		#region Methods

		public override Value	Evaluate (IScope scope, TextWriter output)
		{
			return VoidValue.Instance;
		}

		public override string	ToString ()
		{
			return VoidValue.Instance.ToString ();
		}

		#endregion
	}
}
