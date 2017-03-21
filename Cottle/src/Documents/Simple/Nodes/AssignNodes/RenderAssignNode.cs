using System;
using System.IO;

namespace Cottle.Documents.Simple.Nodes.AssignNodes
{
	class RenderAssignNode : AssignNode
	{
		#region Attributes

		private readonly INode body;

		#endregion

		#region Constructors

		public RenderAssignNode (string name, INode body, StoreMode mode) :
			base (name, mode)
		{
			this.body = body;
		}

		#endregion

		#region Methods

		protected override Value Evaluate (IStore store, TextWriter output)
		{
			Value result;

			using (var buffer = new StringWriter ())
			{
				store.Enter ();

				this.body.Render (store, buffer, out result);

				store.Leave ();

				return buffer.ToString ();
			}
		}

		protected override void SourceSymbol (string name, TextWriter output)
		{
			output.Write (name);
		}

		protected override void SourceValue (ISetting setting, TextWriter output)
		{
			this.body.Source (setting, output);
		}

		#endregion
	}
}
