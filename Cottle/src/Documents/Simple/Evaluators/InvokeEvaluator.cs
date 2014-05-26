using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cottle.Values;

namespace Cottle.Documents.Simple.Evaluators
{
	class InvokeEvaluator : IEvaluator
	{
		#region Events

		public event DocumentError	Error;

		#endregion

		#region Attributes

		private readonly IEvaluator[]	arguments;

		private readonly IEvaluator		caller;

		#endregion

		#region Constructors

		public InvokeEvaluator (IEvaluator caller, IEnumerable<IEvaluator> arguments)
		{
			this.arguments = arguments.ToArray ();
			this.caller = caller;
		}

		#endregion

		#region Methods / Public

		public Value Evaluate (IScope scope, TextWriter output)
		{
			IFunction	function;
			Value		source;
			Value[]		values;

			source = this.caller.Evaluate (scope, output);
			function = source.AsFunction;

			if (function != null)
			{
				values = new Value[this.arguments.Length];

				for (int i = 0; i < this.arguments.Length; ++i)
					values[i] = this.arguments[i].Evaluate (scope, output);

				try
				{
					return function.Execute (values, scope, output);
				}
				catch (Exception exception)
				{
					this.OnError (source, "function call raised an exception", exception);
				}
			}

			return VoidValue.Instance;
		}

		public override string ToString ()
		{
			StringBuilder	builder;
			bool			comma;

			builder = new StringBuilder ();
			builder.Append (this.caller);
			builder.Append ('(');

			comma = false;

			foreach (IEvaluator argument in this.arguments)
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

		#region Methods / Private

		private void OnError (Value source, string message, Exception exception)
		{
			DocumentError	error;

			error = this.Error;

			if (error != null)
				error (source, message, exception);
		}

		#endregion
	}
}
