using System;
using System.IO;

namespace Cottle.Documents.Simple.Evaluators
{
	class ConstantEvaluator : IEvaluator
	{
		#region Attributes

		private readonly Value value;

		#endregion

		#region Constructors

		public ConstantEvaluator (Value value)
		{
			this.value = value;
		}

		#endregion

		#region Methods

		public Value Evaluate (IStore store, TextWriter output)
		{
			return this.value;
		}

		public override string ToString ()
		{
			return this.value.ToString ();
		}

		#endregion
	}
}
