using System;
using System.Collections.Generic;
using System.Linq;

namespace Cottle.Demo
{
    public static class TrimmerCollection
    {
        public const int DefaultIndex = 0;

        private static readonly KeyValuePair<string, Func<string, string>>[] Trimmers =
        {
            new KeyValuePair<string, Func<string, string>>("Remove indent characters (default)",
                DocumentConfiguration.TrimIndentCharacters),
            new KeyValuePair<string, Func<string, string>>("Remove enclosing whitespaces",
                DocumentConfiguration.TrimEnclosingWhitespaces),
            new KeyValuePair<string, Func<string, string>>("Collapse blank characters",
                DocumentConfiguration.TrimRepeatedWhitespaces),
            new KeyValuePair<string, Func<string, string>>("Do not change plain text",
                DocumentConfiguration.TrimNothing)
        };

        public static IEnumerable<string> TrimmerNames
        {
            get { return TrimmerCollection.Trimmers.Select(pair => pair.Key); }
        }

        public static Func<string, string> GetTrimmerFunction(int index)
        {
            if (index >= 0 && index < TrimmerCollection.Trimmers.Length)
                return TrimmerCollection.Trimmers[index].Value;

            return TrimmerCollection.Trimmers[TrimmerCollection.DefaultIndex].Value;
        }

        public static int GetTrimmerIndex(Func<string, string> function)
        {
            var first = TrimmerCollection.Trimmers.Select((pair, index) => new { Index = index, Value = pair.Value })
                .FirstOrDefault(pair => object.ReferenceEquals(function, pair.Value));

            return first?.Index ?? TrimmerCollection.DefaultIndex;
        }
    }
}