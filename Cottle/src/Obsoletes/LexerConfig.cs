using System;

using Cottle.Settings;

namespace Cottle
{
	[Obsolete ("Use CustomSetting")]
	public sealed class LexerConfig : ISetting
	{
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

		private string blockBegin = "{";
		private string blockContinue = "|";
		private string blockEnd = "}";
	}
}
