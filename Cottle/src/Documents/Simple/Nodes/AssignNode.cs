using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Simple.Nodes
{
	abstract class AssignNode : INode
	{
		#region Attributes

		private readonly StoreMode mode;

		private readonly string name;

		#endregion

		#region Constructors

		protected AssignNode (string name, StoreMode mode)
		{
			this.mode = mode;
			this.name = name;
		}

		#endregion

		#region Methods / Abstract

		protected abstract Value Evaluate (IStore store, TextWriter output);

		protected abstract void SourceSymbol (string name, TextWriter output);

		protected abstract void SourceValue (ISetting setting, TextWriter output);

		#endregion

		#region Methods / Public

		public override int GetHashCode ()
		{
			unchecked
			{
				return
					(this.mode.GetHashCode () &	(int)0xFFFF0000) |
					(this.name.GetHashCode () &	(int)0x0000FFFF);
			}
		}

		public bool Render (IStore store, TextWriter output, out Value result)
		{
			store.Set (this.name, this.Evaluate (store, output), this.mode);

			result = VoidValue.Instance;

			return false;
		}

		public void Source (ISetting setting, TextWriter output)
		{
			string keyword;
			string link;

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

			this.SourceSymbol (name, output);

			output.Write (' ');
			output.Write (link);

			this.SourceValue (setting, output);

			output.Write (setting.BlockEnd);
		}

		public override string ToString ()
		{
			return this.name;
		}

		#endregion
	}
}
