using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cottle.Maps;

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

        private static readonly IFunction Absolute = Function.CreatePure1((state, value) => Math.Abs(value.AsNumber));

        private static readonly IFunction Call = Function.CreatePure2((state, caller, arguments) =>
        {
            var fields = new Value[arguments.Fields.Count];
            var function = caller.AsFunction;
            var i = 0;

            if (function == null)
                return Value.Undefined;

            foreach (var pair in arguments.Fields)
                fields[i++] = pair.Value;

            return function.Invoke(state, fields, TextWriter.Null);
        });

        private static readonly IFunction Cast = Function.CreatePure2((state, value, type) =>
        {
            switch (type.AsString)
            {
                case "b":
                case "boolean":
                    return value.AsBoolean;

                case "n":
                case "number":
                    return value.AsNumber;

                case "s":
                case "string":
                    return value.AsString;

                default:
                    return Value.Undefined;
            }
        });

        private static readonly IFunction Cat = Function.CreatePure((state, arguments) =>
        {
            switch (arguments[0].Type)
            {
                case ValueContent.Boolean:
                case ValueContent.Function:
                case ValueContent.Number:
                case ValueContent.String:
                case ValueContent.Void:
                    var builder = new StringBuilder();

                    foreach (var argument in arguments)
                        builder.Append(argument.AsString);

                    return builder.ToString();

                case ValueContent.Map:
                    var concat = new List<Value>();

                    foreach (var argument in arguments)
                    {
                        foreach (var field in argument.Fields)
                            concat.Add(field.Value);
                    }

                    return concat;

                default:
                    throw new InvalidOperationException();

            }
        }, 1, int.MaxValue);

        private static readonly IFunction Ceiling =
            Function.CreatePure1((state, value) => Math.Ceiling(value.AsNumber));

        private static readonly IFunction Char = Function.CreatePure1((state, value) =>
        {
            try
            {
                return char.ConvertFromUtf32((int)value.AsNumber);
            }
            catch
            {
                return '?';
            }
        });

        private static readonly IFunction Compare = Function.CreatePure2((state, lhs, rhs) => lhs.CompareTo(rhs));

        private static readonly IFunction Cosine = Function.CreatePure1((state, value) => Math.Cos(value.AsNumber));

        private static readonly IFunction Cross = Function.CreatePure((state, arguments) =>
        {
            var pairs = new List<KeyValuePair<Value, Value>>();

            foreach (var pair in arguments[0].Fields)
            {
                var insert = true;

                for (var i = 1; i < arguments.Count; ++i)
                {
                    if (!arguments[i].Fields.Contains(pair.Key))
                    {
                        insert = false;

                        break;
                    }
                }

                if (insert)
                    pairs.Add(pair);
            }

            return pairs;
        }, 1, int.MaxValue);

        private static readonly IFunction Default =
            Function.CreatePure2((state, value, fallback) => value.AsBoolean ? value : fallback);

        private static readonly IFunction Defined =
            Function.CreatePure1((state, value) => value != Value.Undefined);

        private static readonly IFunction Except = Function.CreatePure((state, arguments) =>
        {
            var pairs = new List<KeyValuePair<Value, Value>>();

            foreach (var pair in arguments[0].Fields)
            {
                var insert = true;

                for (var i = 1; i < arguments.Count; ++i)
                {
                    if (arguments[i].Fields.Contains(pair.Key))
                    {
                        insert = false;

                        break;
                    }
                }

                if (insert)
                    pairs.Add(pair);
            }

            return pairs;
        }, 1, int.MaxValue);

        private static readonly IFunction Filter = Function.CreatePure((state, arguments) =>
        {
            var forwards = new Value[arguments.Count - 1];
            var function = arguments[1].AsFunction;
            var result = new List<KeyValuePair<Value, Value>>(arguments[0].Fields.Count);

            if (function == null)
                return Value.Undefined;

            foreach (var pair in arguments[0].Fields)
            {
                forwards[0] = pair.Value;

                for (var i = 2; i < arguments.Count; ++i)
                    forwards[i - 1] = arguments[i];

                if (function.Invoke(state, forwards, TextWriter.Null).AsBoolean)
                    result.Add(new KeyValuePair<Value, Value>(pair.Key, pair.Value));
            }

            return result;
        }, 2, int.MaxValue);

        private static readonly IFunction Find = Function.CreatePure((state, arguments) =>
        {
            var offset = arguments.Count > 2 ? (int)arguments[2].AsNumber : 0;
            var search = arguments[1];
            var source = arguments[0];

            if (source.Type == ValueContent.Map)
            {
                var index = 0;

                foreach (var pair in source.Fields)
                {
                    if (++index > offset && pair.Value.Equals(search))
                        return index - 1;
                }

                return -1;
            }

            return source.AsString.IndexOf(search.AsString, offset, StringComparison.Ordinal);
        }, 2, 3);

        private static readonly IFunction Flip = Function.CreatePure1((state, value) =>
        {
            var flip = new KeyValuePair<Value, Value>[value.Fields.Count];
            var i = 0;

            foreach (var pair in value.Fields)
                flip[i++] = new KeyValuePair<Value, Value>(pair.Value, pair.Key);

            return flip;
        });

        private static readonly IFunction Floor = Function.CreatePure1((state, value) => Math.Floor(value.AsNumber));

        private static readonly IFunction Format = Function.CreatePure((state, arguments) =>
        {
            object target;

            var culture = arguments.Count > 2
                ? CultureInfo.GetCultureInfo(arguments[2].AsString)
                : CultureInfo.CurrentCulture;
            var format = arguments[1].AsString;
            var index = format.IndexOf(':');

            switch (index >= 0 ? format.Substring(0, index) : "a")
            {
                case "a":
                    switch (arguments[0].Type)
                    {
                        case ValueContent.Boolean:
                            target = arguments[0].AsBoolean;

                            break;

                        case ValueContent.Function:
                        case ValueContent.Map:
                        case ValueContent.Void:
                            target = null;

                            break;

                        case ValueContent.Number:
                            target = arguments[0].AsNumber;

                            break;

                        case ValueContent.String:
                            target = arguments[0].AsString;

                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    break;

                case "b":
                    target = arguments[0].AsBoolean;

                    break;

                case "d":
                case "du":
                    target = BuiltinFunctions.Epoch.AddSeconds(arguments[0].AsNumber);

                    break;

                case "dl":
                    target = BuiltinFunctions.Epoch.AddSeconds(arguments[0].AsNumber).ToLocalTime();

                    break;

                case "i":
                    target = (long)arguments[0].AsNumber;

                    break;

                case "n":
                    target = arguments[0].AsNumber;

                    break;

                case "s":
                    target = arguments[0].AsString;

                    break;

                default:
                    return Value.Undefined;
            }

            return string.Format(culture, $"{{0:{format.Substring(index + 1)}}}", target);
        }, 2, 3);

        private static readonly IFunction Has = Function.CreatePure((state, arguments) =>
        {
            var source = arguments[0];

            for (var i = 1; i < arguments.Count; ++i)
            {
                if (!source.Fields.Contains(arguments[i]))
                    return false;
            }

            return true;
        }, 1, int.MaxValue);

        private static readonly IFunction Join = Function.CreatePure((state, arguments) =>
        {
            var split = arguments.Count > 1 ? arguments[1].AsString : string.Empty;
            var builder = new StringBuilder();
            var first = true;

            foreach (var pair in arguments[0].Fields)
            {
                if (first)
                    first = false;
                else
                    builder.Append(split);

                builder.Append(pair.Value.AsString);
            }

            return builder.ToString();
        }, 1, 2);

        private static readonly IFunction Length = Function.CreatePure1((state, value) =>
        {
            if (value.Type == ValueContent.Map)
                return value.Fields.Count;

            return value.AsString.Length;
        });

        private static readonly IFunction LowerCase =
            Function.CreatePure1((state, value) => value.AsString.ToLowerInvariant());

        private static readonly IFunction Map = Function.CreatePure((state, arguments) =>
        {
            var forwards = new Value[arguments.Count - 1];
            var function = arguments[1].AsFunction;
            var result = new KeyValuePair<Value, Value>[arguments[0].Fields.Count];

            if (function == null)
                return Value.Undefined;

            var i = 0;

            foreach (var pair in arguments[0].Fields)
            {
                forwards[0] = pair.Value;

                for (var j = 2; j < arguments.Count; ++j)
                    forwards[j - 1] = arguments[j];

                result[i++] =
                    new KeyValuePair<Value, Value>(pair.Key, function.Invoke(state, forwards, TextWriter.Null));
            }

            return result;
        }, 2, int.MaxValue);

        private static readonly IFunction Match = Function.CreatePure((state, arguments) =>
        {
            var match = Regex.Match(arguments[0].AsString, arguments[1].AsString);

            if (!match.Success)
                return Value.Undefined;

            var groups = new List<Value>(match.Groups.Count);

            foreach (Group group in match.Groups)
                groups.Add(group.Value);

            return groups;
        }, 2, 3);

        private static readonly IFunction Maximum = Function.CreatePure((state, arguments) =>
        {
            var max = arguments[0].AsNumber;

            for (var i = 1; i < arguments.Count; ++i)
                max = Math.Max(max, arguments[i].AsNumber);

            return max;
        }, 1, int.MaxValue);

        private static readonly IFunction Minimum = Function.CreatePure((state, arguments) =>
        {
            var min = arguments[0].AsNumber;

            for (var i = 1; i < arguments.Count; ++i)
                min = Math.Min(min, arguments[i].AsNumber);

            return min;
        }, 1, int.MaxValue);

        private static readonly IFunction Ordinal = Function.CreatePure1((state, value) =>
        {
            var str = value.AsString;

            return str.Length > 0 ? char.ConvertToUtf32(str, 0) : 0;
        });

        private static readonly IFunction Power =
            Function.CreatePure2((state, x, y) => Math.Pow(x.AsNumber, y.AsNumber));

        private static readonly IFunction Random = Function.CreatePure((state, arguments) =>
        {
            lock (BuiltinFunctions.RandomGenerator)
            {
                switch (arguments.Count)
                {
                    case 0:
                        return BuiltinFunctions.RandomGenerator.Next();

                    case 1:
                        return BuiltinFunctions.RandomGenerator.Next((int)arguments[0].AsNumber);

                    default:
                        return BuiltinFunctions.RandomGenerator.Next((int)arguments[0].AsNumber,
                            (int)arguments[1].AsNumber);
                }
            }
        }, 0, 2);

        private static readonly IFunction Range = Function.CreatePure((state, arguments) =>
        {
            var start = arguments.Count > 1 ? (int)arguments[0].AsNumber : 0;
            var step = arguments.Count > 2 ? (int)arguments[2].AsNumber : 1;
            var stop = arguments.Count > 1 ? (int)arguments[1].AsNumber : (int)arguments[0].AsNumber;

            if (step == 0)
                return Value.FromMap(EmptyMap.Instance);

            int sign;

            if (step < 0)
            {
                if (start < stop)
                    return Value.FromMap(EmptyMap.Instance);

                sign = 1;
            }
            else
            {
                if (start > stop)
                    return Value.FromMap(EmptyMap.Instance);

                sign = -1;
            }

            return Value.FromGenerator(i => start + step * i, (stop - start + step + sign) / step);
        }, 1, 3);

        private static readonly IFunction Round = Function.CreatePure((state, arguments) =>
        {
            if (arguments.Count > 1)
                return Math.Round(arguments[0].AsNumber, (int)arguments[1].AsNumber);

            return Math.Round(arguments[0].AsNumber);
        }, 1, 2);

        private static readonly IFunction Sine = Function.CreatePure1((state, angle) => Math.Sin(angle.AsNumber));

        private static readonly IFunction Slice = Function.CreatePure((state, arguments) =>
        {
            var source = arguments[0];
            var length = source.Type == ValueContent.Map ? source.Fields.Count : source.AsString.Length;
            var offset = Math.Max(Math.Min((int)arguments[1].AsNumber, length), 0);
            var count = arguments.Count > 2
                ? Math.Max(Math.Min((int)arguments[2].AsNumber, length - offset), 0)
                : length - offset;

            if (source.Type == ValueContent.Map)
            {
                var target = new List<Value>(count);

                using (var enumerator = source.Fields.GetEnumerator())
                {
                    while (offset > 0 && enumerator.MoveNext())
                        --offset;

                    while (count-- > 0 && enumerator.MoveNext())
                        target.Add(enumerator.Current.Value);
                }

                return target;
            }

            return source.AsString.Substring(offset, count);
        }, 2, 3);

        private static readonly IFunction Sort = Function.CreatePure((state, arguments) =>
        {
            var sorted = new List<KeyValuePair<Value, Value>>(arguments[0].Fields);

            if (arguments.Count > 1)
            {
                var callback = arguments[1].AsFunction;

                if (callback == null)
                    return Value.Undefined;

                sorted.Sort((a, b) =>
                    (int)callback.Invoke(state, new[] { a.Value, b.Value }, TextWriter.Null).AsNumber);
            }
            else
                sorted.Sort((a, b) => a.Value.CompareTo(b.Value));

            return sorted;
        }, 1, 2);

        private static readonly IFunction Split = Function.CreatePure2((state, source, separator) =>
            Value.FromEnumerable(source.AsString.Split(new[] { separator.AsString }, StringSplitOptions.None)
                .Select(Value.FromString)));

        private static readonly IFunction Token = Function.CreatePure((state, arguments) =>
        {
            var search = arguments[1].AsString;
            var source = arguments[0].AsString;
            var start = 0;
            var stop = source.IndexOf(search, StringComparison.Ordinal);

            for (var i = Math.Max((int)arguments[2].AsNumber, 0); i > 0; --i)
            {
                if (stop == -1)
                {
                    start = -1;

                    break;
                }

                start = stop + search.Length;
                stop = source.IndexOf(search, start, StringComparison.Ordinal);
            }

            if (arguments.Count < 4)
            {
                if (start < 0)
                    return string.Empty;
                if (stop < 0)
                    return source.Substring(start);
                return source.Substring(start, stop - start);
            }

            if (start < 0)
                return source + search + arguments[3].AsString;
            if (stop < 0)
                return source.Substring(0, start) + arguments[3].AsString;
            return source.Substring(0, start) + arguments[3].AsString + source.Substring(stop);
        }, 3, 4);

        private static readonly IFunction Type =
            Function.CreatePure1((state, value) => value.Type.ToString().ToLowerInvariant());

        private static readonly IFunction Union = Function.CreatePure((state, arguments) =>
        {
            var result = new Dictionary<Value, Value>();

            foreach (var value in arguments)
            {
                foreach (var pair in value.Fields)
                    result[pair.Key] = pair.Value;
            }

            return result;
        }, 0, int.MaxValue);

        private static readonly IFunction UpperCase =
            Function.CreatePure1((state, value) => value.AsString.ToUpperInvariant());

        private static readonly IFunction When = Function.CreatePure((state, arguments) =>
        {
            if (arguments[0].AsBoolean)
                return arguments[1];

            return arguments.Count > 2 ? arguments[2] : Value.Undefined;
        }, 2, 3);

        private static readonly IFunction Xor = Function.CreatePure((state, arguments) =>
        {
            var count = 0;

            foreach (var value in arguments)
            {
                if (value.AsBoolean)
                    ++count;
            }

            return count == 1;
        });

        private static readonly IFunction Zip = Function.CreatePure2((state, first, second) =>
        {
            var firstMap = first.Fields;
            var secondMap = second.Fields;

            using (var enumerator1 = firstMap.GetEnumerator())
            {
                using (var enumerator2 = secondMap.GetEnumerator())
                {
                    var result = new List<KeyValuePair<Value, Value>>(Math.Min(firstMap.Count, secondMap.Count));

                    while (enumerator1.MoveNext() && enumerator2.MoveNext())
                        result.Add(new KeyValuePair<Value, Value>(enumerator1.Current.Value,
                            enumerator2.Current.Value));

                    return result;
                }
            }
        });

        private static readonly Dictionary<string, IFunction> InstanceDictionary = new Dictionary<string, IFunction>
        {
            { "abs", BuiltinFunctions.Absolute },
            { "add", BuiltinOperators.OperatorAdd },
            { "and", BuiltinOperators.OperatorAnd },
            { "call", BuiltinFunctions.Call },
            { "cast", BuiltinFunctions.Cast },
            { "cat", BuiltinFunctions.Cat },
            { "ceil", BuiltinFunctions.Ceiling },
            { "char", BuiltinFunctions.Char },
            { "cmp", BuiltinFunctions.Compare },
            { "cos", BuiltinFunctions.Cosine },
            { "cross", BuiltinFunctions.Cross },
            { "default", BuiltinFunctions.Default },
            { "defined", BuiltinFunctions.Defined },
            { "div", BuiltinOperators.OperatorDiv },
            { "eq", BuiltinOperators.OperatorEqual },
            { "except", BuiltinFunctions.Except },
            { "filter", BuiltinFunctions.Filter },
            { "find", BuiltinFunctions.Find },
            { "flip", BuiltinFunctions.Flip },
            { "floor", BuiltinFunctions.Floor },
            { "format", BuiltinFunctions.Format },
            { "ge", BuiltinOperators.OperatorGreaterEqual },
            { "gt", BuiltinOperators.OperatorGreaterThan },
            { "has", BuiltinFunctions.Has },
            { "join", BuiltinFunctions.Join },
            { "lcase", BuiltinFunctions.LowerCase },
            { "le", BuiltinOperators.OperatorLowerEqual },
            { "len", BuiltinFunctions.Length },
            { "lt", BuiltinOperators.OperatorLowerThan },
            { "map", BuiltinFunctions.Map },
            { "match", BuiltinFunctions.Match },
            { "max", BuiltinFunctions.Maximum },
            { "min", BuiltinFunctions.Minimum },
            { "mod", BuiltinOperators.OperatorMod },
            { "mul", BuiltinOperators.OperatorMul },
            { "ne", BuiltinOperators.OperatorNotEqual },
            { "not", BuiltinOperators.OperatorNot },
            { "or", BuiltinOperators.OperatorOr },
            { "ord", BuiltinFunctions.Ordinal },
            { "pow", BuiltinFunctions.Power },
            { "rand", BuiltinFunctions.Random },
            { "range", BuiltinFunctions.Range },
            { "round", BuiltinFunctions.Round },
            { "sin", BuiltinFunctions.Sine },
            { "slice", BuiltinFunctions.Slice },
            { "sort", BuiltinFunctions.Sort },
            { "split", BuiltinFunctions.Split },
            { "sub", BuiltinOperators.OperatorSub },
            { "token", BuiltinFunctions.Token },
            { "type", BuiltinFunctions.Type },
            { "ucase", BuiltinFunctions.UpperCase },
            { "union", BuiltinFunctions.Union },
            { "when", BuiltinFunctions.When },
            { "xor", BuiltinFunctions.Xor },
            { "zip", BuiltinFunctions.Zip }
        };

        private static readonly Random RandomGenerator = new Random();
    }
}