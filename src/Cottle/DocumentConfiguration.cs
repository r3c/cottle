using System;
using System.Text.RegularExpressions;

namespace Cottle
{
    public struct DocumentConfiguration
    {
        public const string DefaultBlockBegin = "{";
        public const string DefaultBlockContinue = "|";
        public const string DefaultBlockEnd = "}";
        public const char DefaultEscape = '\\';

        private static readonly Regex TrimEnclosingWhitespacesRegex =
            new Regex("^\\s*(.*(?<!\\s))\\s*$", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex TrimFirstAndLastBlankLinesRegex =
            new Regex("^(?:\\n|\\r)(?:\\n|\\r)?[\\t ]*|(?:\\n|\\r)(?:\\n|\\r)?[\\t ]*$", RegexOptions.Compiled);

        private static readonly Regex TrimRepeatedWhitespacesRegex =
            new Regex("\\s{2,}", RegexOptions.Compiled);

        public static readonly Func<string, string> TrimEnclosingWhitespaces =
            s => DocumentConfiguration.TrimEnclosingWhitespacesRegex.Replace(s, m => m.Groups[1].Value);

        public static readonly Func<string, string> TrimFirstAndLastBlankLines =
            s => DocumentConfiguration.TrimFirstAndLastBlankLinesRegex.Replace(s, string.Empty);

        [Obsolete("Please use `TrimFirstAndLastBlankLines` which is equivalent without a misleading name")]
        public static readonly Func<string, string> TrimIndentCharacters =
            DocumentConfiguration.TrimFirstAndLastBlankLines;

        public static readonly Func<string, string> TrimNothing = s => s;

        public static readonly Func<string, string> TrimRepeatedWhitespaces =
            s => DocumentConfiguration.TrimRepeatedWhitespacesRegex.Replace(s, " ");

        public string BlockBegin;

        public string BlockContinue;

        public string BlockEnd;

        public char? Escape;

        public bool NoOptimize;

        public Func<string, string> Trimmer;
    }
}