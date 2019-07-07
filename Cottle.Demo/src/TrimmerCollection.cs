using System.Collections.Generic;
using Cottle.Builtins;
using Cottle.Settings;

namespace Cottle.Demo
{
    public static class TrimmerCollection
    {
        #region Constants

        public const int DefaultIndex = 2;

        #endregion

        #region Attributes

        private static readonly KeyValuePair<string, Trimmer>[] Trimmers =
        {
            new KeyValuePair<string, Trimmer>("Blank characters", BuiltinTrimmers.LeadAndTrailBlankCharacters),
            new KeyValuePair<string, Trimmer>("First and last lines", BuiltinTrimmers.FirstAndLastBlankLines),
            new KeyValuePair<string, Trimmer>("Do not modify text", DefaultSetting.Instance.Trimmer),
            new KeyValuePair<string, Trimmer>("Collapse blank characters", BuiltinTrimmers.CollapseBlankCharacters)
        };

        #endregion

        #region Properties

        public static IEnumerable<string> TrimmerNames
        {
            get
            {
                foreach (var pair in TrimmerCollection.Trimmers)
                    yield return pair.Key;
            }
        }

        #endregion

        #region Methods

        public static Trimmer GetTrimmer(int index)
        {
            if (index >= 0 && index < TrimmerCollection.Trimmers.Length)
                return TrimmerCollection.Trimmers[index].Value;

            return TrimmerCollection.Trimmers[TrimmerCollection.DefaultIndex].Value;
        }

        #endregion
    }
}