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

        public static readonly Func<string, string> TrimEnclosingWhitespaces =
            s => DocumentConfiguration.TrimEnclosingWhitespacesRegex.Replace(s, m => m.Groups[1].Value);

        public static readonly Func<string, string> TrimIndentCharacters =
            s => DocumentConfiguration.TrimIndentCharactersRegex.Replace(s, string.Empty);

        public static readonly Func<string, string> TrimNothing = s => s;

        public static readonly Func<string, string> TrimRepeatedWhitespaces =
            s => DocumentConfiguration.TrimRepeatedWhitespacesRegex.Replace(s, " ");

        private static readonly Regex TrimEnclosingWhitespacesRegex =
            new Regex("^\\s*(.*(?<!\\s))\\s*$", RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex TrimIndentCharactersRegex =
            new Regex("^(?:\\n|\\r)(?:\\n|\\r)?[\\t ]*|(?:\\n|\\r)(?:\\n|\\r)?[\\t ]*$", RegexOptions.Compiled);

        private static readonly Regex TrimRepeatedWhitespacesRegex =
            new Regex("\\s{2,}", RegexOptions.Compiled);

        public string BlockBegin;

        public string BlockContinue;

        public string BlockEnd;

        public char? Escape;

        public bool NoOptimize;

        public Func<string, string> Trimmer;
    }
}