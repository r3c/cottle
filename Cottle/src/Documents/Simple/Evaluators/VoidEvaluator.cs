using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Evaluators
{
	class VoidEvaluator : IEvaluator
	{
		#region Attributes

		public static readonly VoidEvaluator Instance = new VoidEvaluator ();

		#endregion

		#region Methods

		public Value Evaluate (IStore store, TextWriter output)
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
