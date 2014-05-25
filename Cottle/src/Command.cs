using System;

namespace Cottle
{
	class Command
	{
		#region Attributes / Instance

		public string[]			Arguments;

		public Command			Body;

		public CommandBranch[]	Branches;

		public string			Key;

		public ScopeMode		Mode;

		public Command			Next;

		public Expression		Source;

		public string			Text;

		public CommandType		Type;

		public string			Value;

		#endregion
	}
}
