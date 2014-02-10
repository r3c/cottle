using System.Collections.Generic;
using System.IO;

using Cottle.Expressions;
using Cottle.Values;

namespace	Cottle.Nodes
{
	sealed class	ForNode : INode
	{
		#region Attributes

		private readonly INode			body;

		private readonly INode			empty;

		private readonly IExpression	from;

		private readonly NameExpression	key;

		private readonly NameExpression	value;

		#endregion

		#region Constructors

		public	ForNode (IExpression from, NameExpression key, NameExpression value, INode body, INode empty)
		{
			this.body = body;
			this.empty = empty;
			this.from = from;
			this.key = key;
			this.value = value;
		}

		#endregion

		#region Methods

		public bool Render (IScope scope, TextWriter output, out Value result)
		{
			IMap	fields = this.from.Evaluate (scope, output).Fields;

			if (fields.Count > 0)
			{
				foreach (KeyValuePair<Value, Value> pair in fields)
				{
					scope.Enter ();

					if (this.key != null)
						this.key.Set (scope, pair.Key, ScopeMode.Local);

					if (this.value != null)
						this.value.Set (scope, pair.Value, ScopeMode.Local);

					if (this.body.Render (scope, output, out result))
					{
						scope.Leave ();

						return true;
					}

					scope.Leave ();
				}
			}
			else if (this.empty != null)
			{
				scope.Enter ();

				if (this.empty.Render (scope, output, out result))
				{
					scope.Leave ();

					return true;
				}

				scope.Leave ();
			}

			result = VoidValue.Instance;
			
			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			output.Write (setting.BlockBegin);
			output.Write ("for ");

			if (this.key != null)
			{
				output.Write (this.key);
				output.Write (", ");
			}

			output.Write (this.value);
			output.Write (" in ");
			output.Write (this.from);
			output.Write (":");

			this.body.Source (setting, output);

			if (this.empty != null)
			{
				output.Write (setting.BlockContinue);
				output.Write ("empty:");

				this.empty.Source (setting, output);
			}

			output.Write (setting.BlockEnd);
		}

		#endregion
	}
}
