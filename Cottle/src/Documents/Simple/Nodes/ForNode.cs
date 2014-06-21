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

		public bool Render (IStore store, TextWriter output, out Value result)
		{
			IMap	fields;

			fields = this.from.Evaluate (store, output).Fields;

			if (fields.Count > 0)
			{
				foreach (KeyValuePair<Value, Value> pair in fields)
				{
					store.Enter ();

					if (!string.IsNullOrEmpty (this.key))
						store.Set (this.key, pair.Key, StoreMode.Local);

					store.Set (this.value, pair.Value, StoreMode.Local);

					if (this.body.Render (store, output, out result))
					{
						store.Leave ();

						return true;
					}

					store.Leave ();
				}
			}
			else if (this.empty != null)
			{
				store.Enter ();

				if (this.empty.Render (store, output, out result))
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
