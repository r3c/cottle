using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cottle.Exceptions;
using Cottle.Values;

namespace Cottle.Expressions
{
	class CallExpression : IExpression
	{
		#region Attributes

		private readonly List<IExpression>	arguments;

		private readonly IExpression		caller;

		#endregion

		#region Constructors

		public CallExpression (IExpression caller, IEnumerable<IExpression> arguments)
		{
			this.arguments = new List<IExpression> (arguments);
			this.caller = caller;
		}

		#endregion

		#region Methods

		public Value Evaluate (IScope scope, TextWriter output)
		{
			IFunction	function = this.caller.Evaluate (scope, output).AsFunction;
			Value[]		values = new Value[this.arguments.Count];
			int			i = 0;

			if (function != null)
			{
				foreach (IExpression argument in this.arguments)
					values[i++] = argument.Evaluate (scope, output);

				try
				{
					return function.Execute (values, scope, output);
				}
				catch (Exception exception)
				{
					#warning should raise event
					throw new RenderException ("function call raised an exception", exception);
				}
			}

			return VoidValue.Instance;
		}

		public override string ToString ()
		{
			StringBuilder	builder = new StringBuilder ();
			bool			comma = false;

			builder.Append (this.caller);
			builder.Append ('(');

			foreach (IExpression argument in this.arguments)
			{
				if (comma)
					builder.Append (", ");
				else
					comma = true;

				builder.Append (argument);
			}

			builder.Append (')');

			return builder.ToString ();
		}

		#endregion
	}
}
