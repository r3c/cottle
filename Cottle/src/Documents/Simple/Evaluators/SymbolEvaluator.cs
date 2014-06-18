using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Evaluators
{
	class SymbolEvaluator : IEvaluator
	{
		#region Attributes

		private readonly Value	symbol;

		#endregion

		#region Constructors

		public SymbolEvaluator (Value symbol)
		{
			this.symbol = symbol;
		}

		#endregion

		#region Methods

		public Value Evaluate (IScope scope, TextWriter output)
		{
			Value	value;

			if (scope.Get (this.symbol, out value))
				return value;

			return VoidValue.Instance;
		}

		public override string ToString ()
		{
			return this.symbol.AsString;
		}

		#endregion
	}
}
