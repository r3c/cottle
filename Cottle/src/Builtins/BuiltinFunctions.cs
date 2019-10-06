using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cottle.Functions;
using Cottle.Values;

namespace Cottle.Builtins
{
    public static class BuiltinFunctions
    {
        public static IEnumerable<KeyValuePair<string, IFunction>> Instances => BuiltinFunctions.InstanceDictionary;

        public static bool TryGet(string name, out IFunction function)
        {
            return BuiltinFunctions.InstanceDictionary.TryGetValue(name, out function);
        }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly IFunction FunctionAbsolute = new NativeFunction(v => Math.Abs(v[0].AsNumber), 1);

        private static readonly IFunction FunctionCall = new NativeFunction((values, scope, output) =>
        {
            var function = values[0].AsFunction;

            if (function == null)
                return VoidValue.Instance;

            var arguments = new Value[values[1].Fields.Count];
            var i = 0;

            foreach (var pair in values[1].Fields)
                arguments[i++] = pair.Value;

            return function.Execute(arguments, scope, output);
        }, 2);

        private static readonly IFunction FunctionCast = new NativeFunction(values =>
        {
            switch (values[1].AsString)
            {
                case "b":
                case "boolean":
                    return values[0].AsBoolean;

                case "n":
                case "number":
                    return values[0].AsNumber;

                case "s":
                case "string":
                    return values[0].AsString;

                default:
                    return VoidValue.Instance;
            }
        }, 2);

        private static readonly IFunction FunctionCat = new NativeFunction(values =>
        {
            if (values[0].Type == ValueContent.Map)
            {
                var list = new List<Value>(values[0].Fields.Count * 2 + 1);

                foreach (var value in values)
                foreach (var field in value.Fields)
                    list.Add(field.Value);

                return list;
            }

            var builder = new StringBuilder();

            foreach (var value in values)
                builder.Append(value.AsString);

            return builder.ToString();
        }, 1, -1);

        private static readonly IFunction FunctionCeiling = new NativeFunction(v => Math.Ceiling(v[0].AsNumber), 1);

        private static readonly IFunction FunctionChar = new NativeFunction(values =>
        {
            try
            {
                return char.ConvertFromUtf32((int)values[0].AsNumber);
            }
            catch
            {
                return '?';
            }
        }, 1);

        private static readonly IFunction FunctionCompare = new NativeFunction(v => v[0].CompareTo(v[1]), 2);

        private static readonly IFunction FunctionCosine = new NativeFunction(v => Math.Cos((double)v[0].AsNumber), 1);

        private static readonly IFunction FunctionCross = new NativeFunction(values =>
        {
            var pairs = new List<KeyValuePair<Value, Value>>();

            foreach (var pair in values[0].Fields)
            {
                var insert = true;

                for (var i = 1; i < values.Count; ++i)
                    if (!values[i].Fields.Contains(pair.Key))
                    {
                        insert = false;

                        break;
                    }

                if (insert)
                    pairs.Add(pair);
            }

            return pairs;
        }, 1, -1);

        private static readonly IFunction FunctionDefault = new NativeFunction(v => v[0].AsBoolean ? v[0] : v[1], 2);

        private static readonly IFunction FunctionDefined =
            new NativeFunction(values => values[0] != VoidValue.Instance, 1);

        private static readonly IFunction FunctionExcept = new NativeFunction(values =>
        {
            var pairs = new List<KeyValuePair<Value, Value>>();

            foreach (var pair in values[0].Fields)
            {
                var insert = true;

                for (var i = 1; i < values.Count; ++i)
                    if (values[i].Fields.Contains(pair.Key))
                    {
                        insert = false;

                        break;
                    }

                if (insert)
                    pairs.Add(pair);
            }

            return pairs;
        }, 1, -1);

        private static readonly IFunction FunctionFilter = new NativeFunction((values, scope, output) =>
        {
            var callback = values[1].AsFunction;

            if (callback == null)
                return VoidValue.Instance;

            var arguments = new List<Value>(values.Count - 1);
            var result = new List<KeyValuePair<Value, Value>>(values[0].Fields.Count);

            foreach (var pair in values[0].Fields)
            {
                arguments.Clear();
                arguments.Add(pair.Value);

                for (var i = 2; i < values.Count; ++i)
                    arguments.Add(values[i]);

                if (callback.Execute(arguments, scope, output).AsBoolean)
                    result.Add(new KeyValuePair<Value, Value>(pair.Key, pair.Value));
            }

            return result;
        });

        private static readonly IFunction FunctionFind = new NativeFunction(values =>
        {
            var offset = values.Count > 2 ? (int)values[2].AsNumber : 0;
            var search = values[1];
            var source = values[0];

            if (source.Type == ValueContent.Map)
            {
                var index = 0;

                foreach (var pair in source.Fields)
                    if (++index > offset && pair.Value.Equals(search))
                        return index - 1;

                return -1;
            }

            return source.AsString.IndexOf(search.AsString, offset, StringComparison.Ordinal);
        }, 2, 3);

        private static readonly IFunction FunctionFlip = new NativeFunction(values =>
        {
            var flip = new KeyValuePair<Value, Value>[values[0].Fields.Count];
            var i = 0;

            foreach (var pair in values[0].Fields)
                flip[i++] = new KeyValuePair<Value, Value>(pair.Value, pair.Key);

            return flip;
        }, 1);

        private static readonly IFunction FunctionFloor = new NativeFunction(v => Math.Floor(v[0].AsNumber), 1);

        private static readonly IFunction FunctionFormat = new NativeFunction(values =>
        {
            object target;

            var culture = values.Count > 2
                ? CultureInfo.GetCultureInfo(values[2].AsString)
                : CultureInfo.CurrentCulture;
            var format = values[1].AsString;
            var index = format.IndexOf(':');

            switch (index >= 0 ? format.Substring(0, index) : "a")
            {
                case "a":
                    switch (values[0].Type)
                    {
                        case ValueContent.Boolean:
                            target = values[0].AsBoolean;

                            break;

                        case ValueContent.Number:
                            target = values[0].AsNumber;

                            break;

                        case ValueContent.String:
                            target = values[0].AsString;

                            break;

                        default:
                            target = null;

                            break;
                    }

                    break;

                case "b":
                    target = values[0].AsBoolean;

                    break;

                case "d":
                case "du":
                    target = BuiltinFunctions.Epoch.AddSeconds((double)values[0].AsNumber);

                    break;

                case "dl":
                    target = BuiltinFunctions.Epoch.AddSeconds((double)values[0].AsNumber).ToLocalTime();

                    break;

                case "i":
                    target = (long)values[0].AsNumber;

                    break;

                case "n":
                    target = values[0].AsNumber;

                    break;

                case "s":
                    target = values[0].AsString;

                    break;

                default:
                    return VoidValue.Instance;
            }

            return string.Format(culture, "{0:" + format.Substring(index + 1) + "}", target);
        }, 2, 3);

        private static readonly IFunction FunctionHas = new NativeFunction(values =>
        {
            var source = values[0];

            for (var i = 1; i < values.Count; ++i)
                if (!source.Fields.Contains(values[i]))
                    return false;

            return true;
        }, 1, -1);

        private static readonly IFunction FunctionJoin = new NativeFunction(values =>
        {
            var split = values.Count > 1 ? values[1].AsString : string.Empty;
            var builder = new StringBuilder();
            var first = true;

            foreach (var pair in values[0].Fields)
            {
                if (first)
                    first = false;
                else
                    builder.Append(split);

                builder.Append(pair.Value.AsString);
            }

            return builder.ToString();
        }, 1, 2);

        private static readonly IFunction FunctionLength = new NativeFunction(values =>
        {
            if (values[0].Type == ValueContent.Map)
                return values[0].Fields.Count;

            return values[0].AsString.Length;
        }, 1);

        private static readonly IFunction FunctionLowerCase =
            new NativeFunction(v => v[0].AsString.ToLowerInvariant(), 1);

        private static readonly IFunction FunctionMap = new NativeFunction((values, scope, output) =>
        {
            var callback = values[1].AsFunction;

            if (callback == null)
                return VoidValue.Instance;

            var arguments = new List<Value>(values.Count - 1);
            var i = 0;
            var result = new KeyValuePair<Value, Value>[values[0].Fields.Count];

            foreach (var pair in values[0].Fields)
            {
                arguments.Clear();
                arguments.Add(pair.Value);

                for (var j = 2; j < values.Count; ++j)
                    arguments.Add(values[j]);

                result[i++] = new KeyValuePair<Value, Value>(pair.Key, callback.Execute(arguments, scope, output));
            }

            return result;
        }, 2, -1);

        private static readonly IFunction FunctionMatch = new NativeFunction(values =>
        {
            var match = Regex.Match(values[0].AsString, values[1].AsString);

            if (!match.Success)
                return VoidValue.Instance;

            var groups = new List<Value>(match.Groups.Count);

            foreach (Group group in match.Groups)
                groups.Add(group.Value);

            return groups;
        }, 2, 3);

        private static readonly IFunction FunctionMaximum = new NativeFunction(values =>
        {
            var max = values[0].AsNumber;

            for (var i = 1; i < values.Count; ++i)
                max = Math.Max(max, values[i].AsNumber);

            return max;
        }, 1, -1);

        private static readonly IFunction FunctionMinimum = new NativeFunction(values =>
        {
            var min = values[0].AsNumber;

            for (var i = 1; i < values.Count; ++i)
                min = Math.Min(min, values[i].AsNumber);

            return min;
        }, 1, -1);

        private static readonly IFunction FunctionOrd = new NativeFunction(values =>
        {
            var str = values[0].AsString;

            return str.Length > 0 ? char.ConvertToUtf32(str, 0) : 0;
        }, 1);

        private static readonly IFunction FunctionPower =
            new NativeFunction(v => Math.Pow((double)v[0].AsNumber, (double)v[1].AsNumber), 2);

        private static readonly IFunction FunctionRandom = new NativeFunction(values =>
        {
            lock (BuiltinFunctions.Random)
            {
                switch (values.Count)
                {
                    case 0:
                        return BuiltinFunctions.Random.Next();

                    case 1:
                        return BuiltinFunctions.Random.Next((int)values[0].AsNumber);

                    default:
                        return BuiltinFunctions.Random.Next((int)values[0].AsNumber, (int)values[1].AsNumber);
                }
            }
        }, 0, 2);

        private static readonly IFunction FunctionRange = new NativeFunction(values =>
        {
            var start = values.Count > 1 ? (int)values[0].AsNumber : 0;
            var step = values.Count > 2 ? (int)values[2].AsNumber : 1;
            var stop = values.Count > 1 ? (int)values[1].AsNumber : (int)values[0].AsNumber;

            if (step == 0)
                return MapValue.Empty;

            int sign;

            if (step < 0)
            {
                if (start < stop)
                    return MapValue.Empty;

                sign = 1;
            }
            else
            {
                if (start > stop)
                    return MapValue.Empty;

                sign = -1;
            }

            return new MapValue(i => start + step * i, (stop - start + step + sign) / step);
        }, 1, 3);

        private static readonly IFunction FunctionRound = new NativeFunction(values =>
        {
            if (values.Count > 1)
                return Math.Round(values[0].AsNumber, (int)values[1].AsNumber);

            return Math.Round(values[0].AsNumber);
        }, 1, 2);

        private static readonly IFunction FunctionSine = new NativeFunction(v => Math.Sin((double)v[0].AsNumber), 1);

        private static readonly IFunction FunctionSlice = new NativeFunction(values =>
        {
            var source = values[0];
            var length = source.Type == ValueContent.Map ? source.Fields.Count : source.AsString.Length;
            var offset = Math.Max(Math.Min((int)values[1].AsNumber, length), 0);
            var count = values.Count > 2
                ? Math.Max(Math.Min((int)values[2].AsNumber, length - offset), 0)
                : length - offset;

            if (source.Type == ValueContent.Map)
            {
                Value[] target;
                using (var enumerator = source.Fields.GetEnumerator())
                {
                    while (offset-- > 0 && enumerator.MoveNext())
                    {
                    }

                    target = new Value[count];
                    var i = 0;

                    while (count-- > 0 && enumerator.MoveNext())
                        target[i++] = enumerator.Current.Value;
                }

                return target;
            }

            return source.AsString.Substring(offset, count);
        }, 2, 3);

        private static readonly IFunction FunctionSort = new NativeFunction((values, scope, output) =>
        {
            var callback = values.Count > 1 ? values[1].AsFunction : null;
            var sorted = new List<KeyValuePair<Value, Value>>(values[0].Fields);

            if (callback != null)
                sorted.Sort((a, b) => (int)callback.Execute(new[] { a.Value, b.Value }, scope, output).AsNumber);
            else
                sorted.Sort((a, b) => a.Value.CompareTo(b.Value));

            return sorted;
        }, 1, 2);

        private static readonly IFunction FunctionSplit = new NativeFunction(
            v => v[0].AsString.Split(new[] { v[1].AsString }, StringSplitOptions.None).Select(s => new StringValue(s))
                .ToArray(), 2);

        private static readonly IFunction FunctionToken = new NativeFunction(values =>
        {
            var search = values[1].AsString;
            var source = values[0].AsString;
            var start = 0;
            var stop = source.IndexOf(search, StringComparison.Ordinal);

            for (var i = Math.Max((int)values[2].AsNumber, 0); i > 0; --i)
            {
                if (stop == -1)
                {
                    start = -1;

                    break;
                }

                start = stop + search.Length;
                stop = source.IndexOf(search, start, StringComparison.Ordinal);
            }

            if (values.Count < 4)
            {
                if (start < 0)
                    return string.Empty;
                if (stop < 0)
                    return source.Substring(start);
                return source.Substring(start, stop - start);
            }

            if (start < 0)
                return source + search + values[3].AsString;
            if (stop < 0)
                return source.Substring(0, start) + values[3].AsString;
            return source.Substring(0, start) + values[3].AsString + source.Substring(stop);
        }, 3, 4);

        private static readonly IFunction FunctionType =
            new NativeFunction(values => values[0].Type.ToString().ToLowerInvariant(), 1);

        private static readonly IFunction FunctionUnion = new NativeFunction(values =>
        {
            var result = new Dictionary<Value, Value>();

            foreach (var value in values)
            foreach (var pair in value.Fields)
                result[pair.Key] = pair.Value;

            return result;
        }, 0, -1);

        private static readonly IFunction FunctionUpperCase =
            new NativeFunction(v => v[0].AsString.ToUpperInvariant(), 1);

        private static readonly IFunction FunctionWhen = new NativeFunction(values =>
        {
            if (values[0].AsBoolean)
                return values[1];

            return values.Count > 2 ? values[2] : VoidValue.Instance;
        }, 2, 3);

        private static readonly IFunction FunctionXor = new NativeFunction(values =>
        {
            var count = 0;

            foreach (var value in values)
                if (value.AsBoolean)
                    ++count;

            return count == 1;
        });

        private static readonly IFunction FunctionZip = new NativeFunction(values =>
        {
            var map1 = values[0].Fields;
            var map2 = values[1].Fields;

            using (var enumerator1 = map1.GetEnumerator())
            {
                using (var enumerator2 = map2.GetEnumerator())
                {
                    var result = new List<KeyValuePair<Value, Value>>(Math.Min(map1.Count, map2.Count));

                    while (enumerator1.MoveNext() && enumerator2.MoveNext())
                        result.Add(new KeyValuePair<Value, Value>(enumerator1.Current.Value,
                            enumerator2.Current.Value));

                    return result;
                }
            }
        }, 2);

        private static readonly Dictionary<string, IFunction> InstanceDictionary = new Dictionary<string, IFunction>
        {
            { "abs", BuiltinFunctions.FunctionAbsolute },
            { "add", BuiltinOperators.OperatorAdd },
            { "and", BuiltinOperators.OperatorAnd },
            { "call", BuiltinFunctions.FunctionCall },
            { "cast", BuiltinFunctions.FunctionCast },
            { "cat", BuiltinFunctions.FunctionCat },
            { "ceil", BuiltinFunctions.FunctionCeiling },
            { "char", BuiltinFunctions.FunctionChar },
            { "cmp", BuiltinFunctions.FunctionCompare },
            { "cos", BuiltinFunctions.FunctionCosine },
            { "cross", BuiltinFunctions.FunctionCross },
            { "default", BuiltinFunctions.FunctionDefault },
            { "defined", BuiltinFunctions.FunctionDefined },
            { "div", BuiltinOperators.OperatorDiv },
            { "eq", BuiltinOperators.OperatorEqual },
            { "except", BuiltinFunctions.FunctionExcept },
            { "filter", BuiltinFunctions.FunctionFilter },
            { "find", BuiltinFunctions.FunctionFind },
            { "flip", BuiltinFunctions.FunctionFlip },
            { "floor", BuiltinFunctions.FunctionFloor },
            { "format", BuiltinFunctions.FunctionFormat },
            { "ge", BuiltinOperators.OperatorGreaterEqual },
            { "gt", BuiltinOperators.OperatorGreaterThan },
            { "has", BuiltinFunctions.FunctionHas },
            { "join", BuiltinFunctions.FunctionJoin },
            { "lcase", BuiltinFunctions.FunctionLowerCase },
            { "le", BuiltinOperators.OperatorLowerEqual },
            { "len", BuiltinFunctions.FunctionLength },
            { "lt", BuiltinOperators.OperatorLowerThan },
            { "map", BuiltinFunctions.FunctionMap },
            { "match", BuiltinFunctions.FunctionMatch },
            { "max", BuiltinFunctions.FunctionMaximum },
            { "min", BuiltinFunctions.FunctionMinimum },
            { "mod", BuiltinOperators.OperatorMod },
            { "mul", BuiltinOperators.OperatorMul },
            { "ne", BuiltinOperators.OperatorNotEqual },
            { "not", BuiltinOperators.OperatorNot },
            { "or", BuiltinOperators.OperatorOr },
            { "ord", BuiltinFunctions.FunctionOrd },
            { "pow", BuiltinFunctions.FunctionPower },
            { "rand", BuiltinFunctions.FunctionRandom },
            { "range", BuiltinFunctions.FunctionRange },
            { "round", BuiltinFunctions.FunctionRound },
            { "sin", BuiltinFunctions.FunctionSine },
            { "slice", BuiltinFunctions.FunctionSlice },
            { "sort", BuiltinFunctions.FunctionSort },
            { "split", BuiltinFunctions.FunctionSplit },
            { "sub", BuiltinOperators.OperatorSub },
            { "token", BuiltinFunctions.FunctionToken },
            { "type", BuiltinFunctions.FunctionType },
            { "ucase", BuiltinFunctions.FunctionUpperCase },
            { "union", BuiltinFunctions.FunctionUnion },
            { "when", BuiltinFunctions.FunctionWhen },
            { "xor", BuiltinFunctions.FunctionXor },
            { "zip", BuiltinFunctions.FunctionZip }
        };

        private static readonly Random Random = new Random();
    }
}