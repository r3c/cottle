using System.Collections.Generic;
using System.IO;

using Cottle.Values;

namespace	Cottle.Nodes
{
	sealed class	IfNode : INode
	{
		#region Attributes

		private IEnumerable<Branch>	branches;

		private INode				fallback;

		#endregion

		#region Constructors

		public	IfNode (IEnumerable<Branch> branches, INode fallback)
		{
			this.branches = branches;
			this.fallback = fallback;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			bool	halt;

			foreach (Branch branch in this.branches)
			{
				if (branch.Test.Evaluate (scope, output).AsBoolean)
				{
					scope.Enter ();

					halt = branch.Body.Render (scope, output, out result);

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

			foreach (Branch branch in this.branches)
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

				output.Write (branch.Test);
				output.Write (":");

				branch.Body.Source (setting, output);
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

		#region Types

		public class	Branch
		{
			#region Properties

			public INode		Body
			{
				get
				{
					return this.body;
				}
			}

			public IExpression	Test
			{
				get
				{
					return this.test;
				}
			}

			#endregion

			#region Attributes

			private INode		body;

			private IExpression test;

			#endregion

			#region Constructors

			public	Branch (IExpression test, INode body)
			{
				this.body = body;
				this.test = test;
			}

			#endregion
		}

		#endregion
	}
}
