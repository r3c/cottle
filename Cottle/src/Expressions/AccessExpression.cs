using System.IO;
using System.Text;
using Cottle.Values;

namespace Cottle.Expressions
{
	class AccessExpression : IExpression
	{
		#region Attributes

		private readonly IExpression	array;

		private readonly IExpression	index;

		#endregion

		#region Constructors

		public AccessExpression (IExpression array, IExpression index)
		{
			this.array = array;
			this.index = index;
		}

		#endregion

		#region Methods

		public Value Evaluate (IScope scope, TextWriter output)
		{
			Value	source;
			Value	value;

			source = this.array.Evaluate (scope, output);

			if (source.Fields.TryGet (this.index.Evaluate (scope, output), out value))
				return value;

			return VoidValue.Instance;
		}

		public override string ToString ()
		{
			return new StringBuilder ()
				.Append (this.array)
				.Append ('[')
				.Append (this.index)
				.Append (']')
				.ToString ();
		}

		#endregion
	}
}
