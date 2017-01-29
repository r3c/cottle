using System;

namespace Cottle.Settings
{
	public sealed class CustomSetting : ISetting
	{
		#region Properties

		public string BlockBegin
		{
			get
			{
				return this.blockBegin;
			}
			set
			{
				this.blockBegin = value;
			}
		}

		public string BlockContinue
		{
			get
			{
				return this.blockContinue;
			}
			set
			{
				this.blockContinue = value;
			}
		}

		public string BlockEnd
		{
			get
			{
				return this.blockEnd;
			}
			set
			{
				this.blockEnd = value;
			}
		}

		public char Escape
		{
			get
			{
				return this.escape;
			}
			set
			{
				this.escape = value;
			}
		}

		public bool Optimize
		{
			get
			{
				return this.optimize;
			}
			set
			{
				this.optimize = value;
			}
		}

		public Trimmer Trimmer
		{
			get
			{
				return this.trimmer;
			}
			set
			{
				this.trimmer = value;
			}
		}

		#endregion

		#region Attributes

		private string blockBegin = DefaultSetting.Instance.BlockBegin;

		private string blockContinue = DefaultSetting.Instance.BlockContinue;

		private string blockEnd = DefaultSetting.Instance.BlockEnd;

		private char escape = DefaultSetting.Instance.Escape;

		private bool optimize = DefaultSetting.Instance.Optimize;

		private Trimmer trimmer = DefaultSetting.Instance.Trimmer;

		#endregion
	}
}
