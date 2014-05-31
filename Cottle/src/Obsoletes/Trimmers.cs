using System;
using Cottle.Builtins;

namespace Cottle
{
	[Obsolete ("Use Cottle.Builtins.BuiltinTrimmers")]
	public static class Trimmers
	{
		#region Attributes / Public

		public static readonly Trimmer	CollapseBlankCharacters = BuiltinTrimmers.CollapseBlankCharacters;

		public static readonly Trimmer	FirstAndLastBlankLines = BuiltinTrimmers.FirstAndLastBlankLines;

		public static readonly Trimmer	LeadingAndTrailingBlankCharacters = BuiltinTrimmers.LeadAndTrailBlankCharacters;

		#endregion
	}
}
