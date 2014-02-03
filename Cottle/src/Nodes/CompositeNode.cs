using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Values;

namespace	Cottle.Nodes
{
	sealed class	CompositeNode : INode
	{
		#region Attributes

		private IEnumerable<INode>	nodes;

		#endregion

		#region Constructors

		public	CompositeNode (IEnumerable<INode> nodes)
		{
			this.nodes = nodes;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			foreach (INode node in this.nodes)
			{
				if (node.Render (scope, output, out result))
					return true;
			}

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			foreach (INode node in this.nodes)
				node.Source (setting, output);
		}

		#endregion
	}
}
