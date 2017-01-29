using System.IO;
using System.Text;
using Cottle.Values;

namespace Cottle.Documents.Simple.Evaluators
{
	class AccessEvaluator : IEvaluator
	{
		#region Attributes

		private readonly IEvaluator source;

		private readonly IEvaluator subscript;

		#endregion

		#region Constructors

		public AccessEvaluator (IEvaluator source, IEvaluator subscript)
		{
			this.source = source;
			this.subscript = subscript;
		}

		#endregion

		#region Methods

		public Value Evaluate (IStore store, TextWriter output)
		{
			Value key;
			Value map;
			Value value;

			key = this.subscript.Evaluate (store, output);
			map = this.source.Evaluate (store, output);

			if (map.Fields.TryGet (key, out value))
				return value;

			return VoidValue.Instance;
		}

		public override string ToString ()
		{
			return new StringBuilder ()
				.Append (this.source)
				.Append ('[')
				.Append (this.subscript)
				.Append (']')
				.ToString ();
		}

		#endregion
	}
}
