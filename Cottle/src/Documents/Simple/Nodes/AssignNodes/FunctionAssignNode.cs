using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes.AssignNodes
{
	class FunctionAssignNode : AssignNode, IFunction
	{
		#region Attributes

		private readonly string[] arguments;

		private readonly INode body;

		#endregion

		#region Constructors

		public FunctionAssignNode (string name, IEnumerable<string> arguments, INode body, StoreMode mode) :
			base (name, mode)
		{
			this.arguments = arguments.ToArray ();
			this.body = body;
		}

		#endregion

		#region Methods / Public

		public int CompareTo (IFunction other)
		{
			return object.ReferenceEquals (this, other) ? 0 : 1;
		}

		public bool Equals (IFunction other)
		{
			return this.CompareTo (other) == 0;
		}

		public override bool Equals (object obj)
		{
			IFunction other = obj as IFunction;

			return other != null && this.Equals (other);
		}

		public Value Execute (IList<Value> arguments, IStore store, TextWriter output)
		{
			Value result;

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
					(base.GetHashCode () &	(int)0x000000FF);
			}
		}

		#endregion

		#region Methods / Protected

		protected override Value Evaluate (IStore store, TextWriter output)
		{
			return new FunctionValue (this);
		}

		protected override void SourceSymbol (string name, TextWriter output)
		{
			bool comma = false;

			output.Write (name);
			output.Write ('(');

			foreach (string argument in this.arguments)
			{
				if (comma)
					output.Write (", ");
				else
					comma = true;

				output.Write (argument);
			}

			output.Write (' ');
		}

		protected override void SourceValue (ISetting setting, TextWriter output)
		{
			output.Write (':');

			this.body.Source (setting, output);
		}

		#endregion
	}
}
