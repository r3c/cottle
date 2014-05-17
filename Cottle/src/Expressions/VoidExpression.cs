using System.IO;
using Cottle.Values;

namespace Cottle.Expressions
{
	class VoidExpression : IExpression
	{
		#region Attributes

		public static readonly VoidExpression	Instance = new VoidExpression ();

		#endregion

		#region Methods

		public Value Evaluate (IScope scope, TextWriter output)
		{
			return VoidValue.Instance;
		}

		public override string ToString ()
		{
			return VoidValue.Instance.ToString ();
		}

		#endregion
	}
}
