using System;

namespace Cottle
{
	class Command
	{
		#region Attributes / Instance

		public string[]		Arguments;

		public Command		Body;

		public string		Key;

		public ScopeMode	Mode;

		public string		Name;

		public Command		Next;

		public Expression	Operand;

		public string		Text;

		public CommandType	Type;

		#endregion
	}
}
