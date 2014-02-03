using System.IO;
using System.Text;

using Cottle.Expressions.Abstracts;
using Cottle.Values;

namespace	Cottle.Expressions
{
	class	AccessExpression : Expression
	{
		#region Attributes

		private IExpression array;

		private IExpression index;

		#endregion

		#region Constructors

		public	AccessExpression (IExpression array, IExpression index)
		{
			this.array = array;
			this.index = index;
		}

		#endregion

		#region Methods

		public override Value	Evaluate (IScope scope, TextWriter output)
		{
			Value	array = this.array.Evaluate (scope, output);
			Value	value;

			if (array.Fields.TryGet (this.index.Evaluate (scope, output), out value))
				return value;

			return VoidValue.Instance;
		}

		public override string	ToString ()
		{
			StringBuilder	builder;

			builder = new StringBuilder ();
			builder.Append (this.array);
			builder.Append ('[');
			builder.Append (this.index);
			builder.Append (']');

			return builder.ToString ();
		}

		#endregion
	}
}
