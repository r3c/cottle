using System;
using System.Collections.Generic;

using Cottle.Cleaners;
using Cottle.Settings;

namespace Cottle.Demo
{
	public static class TrimmerCollection
	{
		#region Constants
		
		public const int	DEFAULT_INDEX = 2;
		
		#endregion

		#region Properties
		
		public static IEnumerable<string>	TrimmerNames
		{
			get
			{
				foreach (KeyValuePair<string, Trimmer> pair in TrimmerCollection.trimmers)
					yield return pair.Key;
			}
		}
		
		#endregion

		#region Attributes

		private static readonly KeyValuePair<string, Trimmer>[]	trimmers = new KeyValuePair<string, Trimmer>[]
		{
			new KeyValuePair<string, Trimmer> ("Blank characters",			Trimmers.LeadingAndTrailingBlankCharacters),
			new KeyValuePair<string, Trimmer> ("First and last lines",		Trimmers.FirstAndLastBlankLines),
			new KeyValuePair<string, Trimmer> ("Do not modify text",		DefaultSetting.Instance.Trimmer),
			new KeyValuePair<string, Trimmer> ("Collapse blank characters",	Trimmers.CollapseBlankCharacters)
		};

		#endregion
		
		#region Methods
		
		public static Trimmer	GetTrimmer (int index)
		{
			if (index >= 0 && index < TrimmerCollection.trimmers.Length)
				return TrimmerCollection.trimmers[index].Value;

			return TrimmerCollection.trimmers[TrimmerCollection.DEFAULT_INDEX].Value;
		}

		#endregion
	}
}
