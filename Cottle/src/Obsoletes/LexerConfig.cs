using System;

using Cottle.Settings;

namespace Cottle
{
	[Obsolete ("Use Cottle.Settings.CustomSetting")]
	public sealed class LexerConfig : ISetting
	{
		public string BlockBegin
		{
			get
			{
				return this.setting.BlockBegin;
			}
			set
			{
				this.setting.BlockBegin = value;
			}
		}
		public string BlockContinue
		{
			get
			{
				return this.setting.BlockContinue;
			}
			set
			{
				this.setting.BlockContinue = value;
			}
		}

		public string BlockEnd
		{
			get
			{
				return this.setting.BlockEnd;
			}
			set
			{
				this.setting.BlockEnd = value;
			}
		}

		public char Escape
		{
			get
			{
				return this.setting.Escape;
			}
			set
			{
				this.setting.Escape = value;
			}
		}

		public bool Optimize
		{
			get
			{
				return this.setting.Optimize;
			}
		}

		public Trimmer Trimmer
		{
			get
			{
				return DefaultSetting.Instance.Trimmer;
			}
		}

		public ICleaner Cleaner
		{
			get
			{
				throw new InvalidOperationException ();
			}
		}

		private CustomSetting setting = new CustomSetting ();
	}
}
