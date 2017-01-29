using System;

namespace Cottle.Demo
{
	public class NodeData
	{
		#region Properties

		public int ImageIndex
		{
			get
			{
				return (int)this.value.Type;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public Value Value
		{
			get
			{
				return this.value;
			}
		}

		#endregion

		#region Attributes

		private readonly string key;

		private readonly Value value;

		#endregion

		#region Constructors

		public NodeData (string key, Value value)
		{
			this.key = key;
			this.value = value;
		}

		#endregion
	}
}
