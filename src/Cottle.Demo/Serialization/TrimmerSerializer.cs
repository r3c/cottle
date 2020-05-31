using System;
using System.Collections.Generic;
using System.Linq;

namespace Cottle.Demo.Serialization
{
    public static class TrimmerSerializer
    {
        public const int DefaultIndex = 0;

        private static readonly IReadOnlyList<(string, Func<string, string>)> Trimmers = new[]
        {
            ("Remove first and last blank lines (default)", DocumentConfiguration.TrimFirstAndLastBlankLines),
            ("Remove enclosing whitespaces", DocumentConfiguration.TrimEnclosingWhitespaces),
            ("Collapse blank characters", DocumentConfiguration.TrimRepeatedWhitespaces),
            ("Do not change plain text", DocumentConfiguration.TrimNothing)
        };

        public static IEnumerable<string> TrimmerNames => TrimmerSerializer.Trimmers.Select(pair => pair.Item1);

        public static Func<string, string> GetFunction(int index)
        {
            if (index >= 0 && index < TrimmerSerializer.Trimmers.Count)
                return TrimmerSerializer.Trimmers[index].Item2;

            return TrimmerSerializer.Trimmers[TrimmerSerializer.DefaultIndex].Item2;
        }

        public static int GetIndex(Func<string, string> function)
        {
            var first = TrimmerSerializer.Trimmers.Select((pair, index) => new { Index = index, Value = pair.Item2 })
                .FirstOrDefault(pair => object.ReferenceEquals(function, pair.Value));

            return first?.Index ?? TrimmerSerializer.DefaultIndex;
        }
    }
}