using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	class AssignFunctionNode : IFunction, INode
	{
		#region Attributes

		private readonly string[]	arguments;

		private readonly INode		body;

		private readonly StoreMode	mode;

		private readonly string		name;

		#endregion

		#region Constructors

		public AssignFunctionNode (string name, IEnumerable<string> arguments, INode body, StoreMode mode)
		{
			this.arguments = arguments.ToArray ();
			this.body = body;
			this.mode = mode;
			this.name = name;
		}

		#endregion

		#region Methods

		public int CompareTo (IFunction other)
		{
			return object.ReferenceEquals (this, other) ? 0 : 1;
		}

		public bool	Equals (IFunction other)
		{
			return this.CompareTo (other) == 0;
		}

		public override bool Equals (object obj)
		{
			IFunction	other = obj as IFunction;

			return other != null && this.Equals (other);
		}

		public Value Execute (IList<Value> arguments, IStore store, TextWriter output)
		{
			Value	result;

			store.Enter ();

			for (int i = 0; i < this.arguments.Length; ++i)
				store.Set (this.arguments[i], i < arguments.Count ? arguments[i] : VoidValue.Instance, StoreMode.Local);

			this.body.Render (store, output, out result);

			store.Leave ();

			return result;
		}

		public override int GetHashCode ()
		{
			unchecked
			{
				return
					(this.body.GetHashCode () &	(int)0xFFFFFF00) |
					(this.mode.GetHashCode () &	(int)0x000000C0) |
					(this.name.GetHashCode () &	(int)0x0000003F);
			}
		}

		public bool Render (IStore store, TextWriter output, out Value result)
		{
			store.Set (this.name, new FunctionValue (this), mode);

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			bool	comma;
			string	keyword;
			string	link;

			switch (this.mode)
			{
				case StoreMode.Local:
					keyword = "declare";
					link = "as";

					break;

				default:
					keyword = "set";
					link = "to";

					break;
			}

			output.Write (setting.BlockBegin);
			output.Write (keyword);
			output.Write (' ');
			output.Write (this.name);
			output.Write ('(');

			comma = false;

			foreach (string argument in this.arguments)
			{
				if (comma)
					output.Write (", ");
				else
					comma = true;

				output.Write (argument);
			}

			output.Write (") ");
			output.Write (link);
			output.Write (": ");

			this.body.Source (setting, output);

			output.Write (setting.BlockEnd);
		}

		public override string ToString ()
		{
			return this.name;
		}

		#endregion
	}
}
