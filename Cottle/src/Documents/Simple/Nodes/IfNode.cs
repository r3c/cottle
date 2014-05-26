using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	class IfNode : INode
	{
		#region Attributes

		private readonly KeyValuePair<IEvaluator, INode>[]	branches;

		private readonly INode								fallback;

		#endregion

		#region Constructors

		public IfNode (IEnumerable<KeyValuePair<IEvaluator, INode>> branches, INode fallback)
		{
			this.branches = branches.ToArray ();
			this.fallback = fallback;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			bool	halt;

			foreach (KeyValuePair<IEvaluator, INode> branch in this.branches)
			{
				if (branch.Key.Evaluate (scope, output).AsBoolean)
				{
					scope.Enter ();

					halt = branch.Value.Render (scope, output, out result);

					scope.Leave ();

					return halt;
				}
			}

			if (this.fallback != null)
			{
				scope.Enter ();

				halt = this.fallback.Render (scope, output, out result);

				scope.Leave ();

				return halt;
			}

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			bool	first;
			
			first = true;

			foreach (KeyValuePair<IEvaluator, INode> branch in this.branches)
			{
				if (first)
				{
					output.Write (setting.BlockBegin);
					output.Write ("if ");

					first = false;
				}
				else
				{
					output.Write (setting.BlockContinue);
					output.Write ("elif ");
				}

				output.Write (branch.Key);
				output.Write (":");

				branch.Value.Source (setting, output);
			}

			if (this.fallback != null)
			{
				output.Write (setting.BlockContinue);
				output.Write ("else:");

				this.fallback.Source (setting, output);
			}

			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
