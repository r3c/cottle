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
            new Regex(@"^\s*(.*(?<!\s))\s*\z", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex TrimFirstAndLastBlankLinesRegex =
            new Regex(@"^(?:\n|\r(?!\n)|\r\n)[\t ]*|(?:\n|\r(?!\n)|\r\n)[\t ]*\z", RegexOptions.Compiled);

        private static readonly Regex TrimRepeatedWhitespacesRegex =
            new Regex(@"\s{2,}", RegexOptions.Compiled);

        public static readonly Func<string, string> TrimEnclosingWhitespaces =
            s => TrimEnclosingWhitespacesRegex.Replace(s, m => m.Groups[1].Value);

        public static readonly Func<string, string> TrimFirstAndLastBlankLines =
            s => TrimFirstAndLastBlankLinesRegex.Replace(s, string.Empty);

        [Obsolete("Please use `TrimFirstAndLastBlankLines` which is equivalent without a misleading name")]
        public static readonly Func<string, string> TrimIndentCharacters = TrimFirstAndLastBlankLines;

        public static readonly Func<string, string> TrimNothing = s => s;

        public static readonly Func<string, string> TrimRepeatedWhitespaces = s => TrimRepeatedWhitespacesRegex.Replace(s, " ");

        public static readonly Func<string, string> DefaultTrimmer = TrimFirstAndLastBlankLines;

        public string? BlockBegin;

        public string? BlockContinue;

        public string? BlockEnd;

        public char? Escape;

        public bool NoOptimize;

        public TimeSpan? Timeout;

        public Func<string, string>? Trimmer;
    }
}