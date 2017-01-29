using System;

namespace Cottle.Settings
{
	public sealed class DefaultSetting : ISetting
	{
		#region Properties / Instance

		public string BlockBegin
		{
			get
			{
				return "{";
			}
		}

		public string BlockContinue
		{
			get
			{
				return "|";
			}
		}

		public string BlockEnd
		{
			get
			{
				return "}";
			}
		}

		public char Escape
		{
			get
			{
				return '\\';
			}
		}

		public bool Optimize
		{
			get
			{
				return true;
			}
		}

		public Trimmer Trimmer
		{
			get
			{
				return (t) => t;
			}
		}

		#endregion
		
		#region Properties / Static
		
		public static DefaultSetting Instance
		{
			get
			{
				return DefaultSetting.instance;
			}
		}
		
		#endregion

		#region Attributes

		private static readonly DefaultSetting instance = new DefaultSetting ();

		#endregion
	}
}
