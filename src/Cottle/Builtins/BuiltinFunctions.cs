using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cottle.Functions;
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

        private static readonly Random RandomGenerator = new Random();

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

        private static int FindCallback(Value source, Value search, int offset)
        {
            if (source.Type != ValueContent.Map)
                return source.AsString.IndexOf(search.AsString, offset, StringComparison.Ordinal);

            var index = 0;

            foreach (var pair in source.Fields)
            {
                if (++index > offset && pair.Value.Equals(search))
                    return index - 1;
            }

            return -1;
        }

        private static readonly IFunction Find = new FiniteFunction(true, null, null,
            (s, a0, a1, _) => BuiltinFunctions.FindCallback(a0, a1, 0),
            (s, a0, a1, a2, _) => BuiltinFunctions.FindCallback(a0, a1, (int)a2.AsNumber));

        private static readonly IFunction Flip = Function.CreatePure1((state, value) =>
        {
            var flip = new KeyValuePair<Value, Value>[value.Fields.Count];
            var i = 0;

            foreach (var pair in value.Fields)
                flip[i++] = new KeyValuePair<Value, Value>(pair.Value, pair.Key);

            return flip;
        });

        private static readonly IFunction Floor = Function.CreatePure1((state, value) => Math.Floor(value.AsNumber));

        private static Value FormatCallback(Value subject, string format, IFormatProvider formatProvider)
        {
            object? target;

            var index = format.IndexOf(':');

            switch (index >= 0 ? format.Substring(0, index) : "a")
            {
                case "a":
                    switch (subject.Type)
                    {
                        case ValueContent.Boolean:
                            target = subject.AsBoolean;

                            break;

                        case ValueContent.Function:
                        case ValueContent.Map:
                        case ValueContent.Void:
                            target = null;

                            break;

                        case ValueContent.Number:
                            target = subject.AsNumber;

                            break;

                        case ValueContent.String:
                            target = subject.AsString;

                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    break;

                case "b":
                    target = subject.AsBoolean;

                    break;

                case "d":
                case "du":
                    target = BuiltinFunctions.Epoch.AddSeconds(subject.AsNumber);

                    break;

                case "dl":
                    target = BuiltinFunctions.Epoch.AddSeconds(subject.AsNumber).ToLocalTime();

                    break;

                case "i":
                    target = (long)subject.AsNumber;

                    break;

                case "n":
                    target = subject.AsNumber;

                    break;

                case "s":
                    target = subject.AsString;

                    break;

                default:
                    return Value.Undefined;
            }

            return string.Format(formatProvider, $"{{0:{format.Substring(index + 1)}}}", target);
        }

        private static readonly IFunction Format = new FiniteFunction(true, null, null,
            (s, a0, a1, _) => BuiltinFunctions.FormatCallback(a0, a1.AsString, CultureInfo.CurrentCulture),
            (s, a0, a1, a2, _) =>
            {
                CultureInfo cultureInfo;

                try
                {
                    cultureInfo = CultureInfo.GetCultureInfo(a2.AsString);
                }
                catch (CultureNotFoundException)
                {
                    return Value.Undefined;
                }

                return BuiltinFunctions.FormatCallback(a0, a1.AsString, cultureInfo);
            });

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

        private static string JoinCallback(Value input, string separator)
        {
            var builder = new StringBuilder();
            var first = true;

            foreach (var pair in input.Fields)
            {
                if (first)
                    first = false;
                else
                    builder.Append(separator);

                builder.Append(pair.Value.AsString);
            }

            return builder.ToString();
        }

        private static readonly IFunction Join = new FiniteFunction(true, null,
            (s, a0, _) => BuiltinFunctions.JoinCallback(a0, string.Empty),
            (s, a0, a1, _) => BuiltinFunctions.JoinCallback(a0, a1.AsString), null);

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

        private static readonly IFunction Match = Function.CreatePure2((state, subject, pattern) =>
        {
            var match = Regex.Match(subject.AsString, pattern.AsString);

            if (!match.Success)
                return Value.Undefined;

            var groups = new List<Value>(match.Groups.Count);

            foreach (Group group in match.Groups)
                groups.Add(group.Value);

            return groups;
        });

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

        private static readonly IFunction Random = new FiniteFunction(true, (s, _) =>
        {
            lock (BuiltinFunctions.RandomGenerator)
            {
                return BuiltinFunctions.RandomGenerator.Next();
            }
        }, (s, maxValue, _) =>
        {
            lock (BuiltinFunctions.RandomGenerator)
            {
                return BuiltinFunctions.RandomGenerator.Next((int)maxValue.AsNumber);
            }
        }, (s, minValue, maxValue, _) =>
        {
            lock (BuiltinFunctions.RandomGenerator)
            {
                return BuiltinFunctions.RandomGenerator.Next((int)minValue.AsNumber, (int)maxValue.AsNumber);
            }
        }, null);

        private static Value RangeCallback(int start, int stop, int step)
        {
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
        }

        private static readonly IFunction Range = new FiniteFunction(true, null,
            (s, a0, _) => BuiltinFunctions.RangeCallback(0, (int)a0.AsNumber, 1),
            (s, a0, a1, _) => BuiltinFunctions.RangeCallback((int)a0.AsNumber, (int)a1.AsNumber, 1),
            (s, a0, a1, a2, _) => BuiltinFunctions.RangeCallback((int)a0.AsNumber, (int)a1.AsNumber, (int)a2.AsNumber));

        private static readonly IFunction Round = new FiniteFunction(true, null,
            (s, a0, _) => Math.Round(a0.AsNumber),
            (s, a0, a1, _) => Math.Round(a0.AsNumber, (int)a1.AsNumber), null);

        private static readonly IFunction Sine = Function.CreatePure1((state, angle) => Math.Sin(angle.AsNumber));

        private static Value SliceCallback(Value source, int unsafeIndex, int unsafeCount)
        {
            var length = source.Type == ValueContent.Map ? source.Fields.Count : source.AsString.Length;
            var index = Math.Max(Math.Min(unsafeIndex, length), 0);
            var count = Math.Max(Math.Min(unsafeCount, length - index), 0);

            if (source.Type != ValueContent.Map)
                return source.AsString.Substring(index, count);

            var target = new List<Value>(count);

            using (var enumerator = source.Fields.GetEnumerator())
            {
                while (index > 0 && enumerator.MoveNext())
                    --index;

                while (count-- > 0 && enumerator.MoveNext())
                    target.Add(enumerator.Current.Value);
            }

            return target;
        }

        private static readonly IFunction Slice = new FiniteFunction(true, null, null,
            (s, a0, a1, _) => BuiltinFunctions.SliceCallback(a0, (int)a1.AsNumber, int.MaxValue),
            (s, a0, a1, a2, _) => BuiltinFunctions.SliceCallback(a0, (int)a1.AsNumber, (int)a2.AsNumber));

        private static List<KeyValuePair<Value, Value>> SortCallback(IMap map,
            Comparison<KeyValuePair<Value, Value>> comparison)
        {
            var sorted = new List<KeyValuePair<Value, Value>>(map);

            sorted.Sort(comparison);

            return sorted;
        }

        private static readonly IFunction Sort = new FiniteFunction(true, null,
            (s, a0, _) => BuiltinFunctions.SortCallback(a0.Fields, (a, b) => a.Value.CompareTo(b.Value)),
            (s, a0, a1, _) =>
            {
                if (a1.Type != ValueContent.Function)
                    return Value.Undefined;

                var comparison = a1.AsFunction;

                return BuiltinFunctions.SortCallback(a0.Fields, (a, b) =>
                    (int)comparison.Invoke(s, new[] { a.Value, b.Value }, TextWriter.Null).AsNumber);
            }, null);

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

        private static readonly IFunction When = new FiniteFunction(true, null, null,
            (s, condition, truthy, _) => condition.AsBoolean ? truthy : Value.Undefined,
            (s, condition, truthy, falsy, _) => condition.AsBoolean ? truthy : falsy);

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
    }
}