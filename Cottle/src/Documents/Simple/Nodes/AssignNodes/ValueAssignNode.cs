using System;
using System.IO;

namespace Cottle.Documents.Simple.Nodes.AssignNodes
{
	class ValueAssignNode : AssignNode
	{
		#region Attributes

		private readonly IEvaluator expression;

		#endregion

		#region Constructors

		public ValueAssignNode (string name, IEvaluator expression, StoreMode mode) :
			base (name, mode)
		{
			this.expression = expression;
		}

		#endregion

		#region Methods

		protected override Value Evaluate (IStore store, TextWriter output)
		{
			return this.expression.Evaluate (store, output);
		}

		protected override void SourceSymbol (string name, TextWriter output)
		{
			output.Write (name);
		}

		protected override void SourceValue (ISetting setting, TextWriter output)
		{
			output.Write (this.expression);
		}

		#endregion
	}
}
