using System.Collections.Generic;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	class ForNode : INode
	{
		#region Attributes

		private readonly INode		body;

		private readonly INode		empty;

		private readonly IEvaluator	from;

		private readonly string		key;

		private readonly string		value;

		#endregion

		#region Constructors

		public ForNode (IEvaluator from, string key, string value, INode body, INode empty)
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
			IMap	fields;

			fields = this.from.Evaluate (scope, output).Fields;

			if (fields.Count > 0)
			{
				foreach (KeyValuePair<Value, Value> pair in fields)
				{
					scope.Enter ();

					if (!string.IsNullOrEmpty (this.key))
						scope.Set (this.key, pair.Key, ScopeMode.Local);

					scope.Set (this.value, pair.Value, ScopeMode.Local);

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

			if (!string.IsNullOrEmpty (this.key))
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
