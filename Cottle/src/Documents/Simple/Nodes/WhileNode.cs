using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	class WhileNode : INode
	{
		#region Attributes

		private readonly INode body;

		private readonly IEvaluator condition;

		#endregion

		#region Constructors

		public WhileNode (IEvaluator condition, INode body)
		{
			this.body = body;
			this.condition = condition;
		}

		#endregion

		#region Methods

		public bool Render (IStore store, TextWriter output, out Value result)
		{
			while (this.condition.Evaluate (store, output).AsBoolean)
			{
				store.Enter ();

				if (this.body.Render (store, output, out result))
				{
					store.Leave ();

					return true;
				}

				store.Leave ();
			}

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			output.Write (setting.BlockBegin);
			output.Write ("while ");
			output.Write (this.condition);
			output.Write (":");

			this.body.Source (setting, output);

			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
