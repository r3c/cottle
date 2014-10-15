using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Cottle.Functions;
using Cottle.Values;

namespace Cottle.Builtins
{
	public static class BuiltinFunctions
	{
		#region Properties

		public static IEnumerable<KeyValuePair<string, IFunction>>	Instances
		{
			get
			{
				return BuiltinFunctions.instances;
			}
		}

		#endregion

		#region Attributes

		private static readonly DateTime	epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static readonly IFunction	functionAbsolute = new NativeFunction ((v) => Math.Abs (v[0].AsNumber), 1);

		private static readonly IFunction	functionAnd = new NativeFunction ((values) =>
		{
			foreach (Value value in values)
			{
				if (!value.AsBoolean)
					return false;
			}

			return true;
		});

		private static readonly IFunction	functionCall = new NativeFunction ((values, scope, output) =>
		{
			Value[]		arguments;
			IFunction	function;
			int			i;

			function = values[0].AsFunction;

			if (function == null)
				return VoidValue.Instance;

			arguments = new Value[values[1].Fields.Count];
			i = 0;

			foreach (KeyValuePair<Value, Value> pair in values[1].Fields)
				arguments[i++] = pair.Value;

			return function.Execute (arguments, scope, output);
		}, 2);

		private static readonly IFunction	functionCast = new NativeFunction ((values) =>
		{
			switch (values[1].AsString)
			{
				case "b":
					return values[0].AsBoolean;

				case "n":
					return values[0].AsNumber;

				case "s":
					return values[0].AsString;

				default:
					return VoidValue.Instance;
			}
		}, 2);

		private static readonly IFunction	functionCat = new NativeFunction ((values) =>
		{
			StringBuilder						builder;
			List<KeyValuePair<Value, Value>>	list;

			if (values[0].Type == ValueContent.Map)
			{
				list = new List<KeyValuePair<Value, Value>> (values[0].Fields.Count * 2 + 1);

				foreach (Value value in values)
					list.AddRange (value.Fields);

				return list;
			}
			else
			{
				builder = new StringBuilder ();

				foreach (Value value in values)
					builder.Append (value.AsString);

				return builder.ToString ();
			}
		}, 1, -1);

		private static readonly IFunction	functionCeiling = new NativeFunction ((v) => Math.Ceiling (v[0].AsNumber), 1);

		private static readonly IFunction	functionChar = new NativeFunction ((values) =>
		{
			try
			{
				return char.ConvertFromUtf32 ((int)values[0].AsNumber);
			}
			catch
			{
				return '?';
			}
		}, 1);

		private static readonly IFunction	functionCompare = new NativeFunction ((v) => v[0].CompareTo (v[1]), 2);

		private static readonly IFunction	functionCosine = new NativeFunction ((v) => Math.Cos ((double)v[0].AsNumber), 1);

		private static readonly IFunction	functionCross = new NativeFunction ((values) =>
		{
			bool								insert;
			List<KeyValuePair<Value, Value>>	pairs;

			pairs = new List<KeyValuePair<Value, Value>> ();

			foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
			{
				insert = true;

				for (int i = 1; i < values.Count; ++i)
				{
					if (!values[i].Fields.Contains (pair.Key))
					{
						insert = false;

						break;
					}
				}

				if (insert)
					pairs.Add (pair);
			}

			return pairs;
		}, 1, -1);

		private static readonly IFunction	functionDefault = new NativeFunction ((v) => v[0].AsBoolean ? v[0] : v[1], 2);

		private static readonly IFunction	functionExcept = new NativeFunction ((values) =>
		{
			bool								insert;
			List<KeyValuePair<Value, Value>>	pairs;

			pairs = new List<KeyValuePair<Value, Value>> ();

			foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
			{
				insert = true;

				for (int i = 1; i < values.Count; ++i)
				{
					if (values[i].Fields.Contains (pair.Key))
					{
						insert = false;

						break;
					}
				}

				if (insert)
					pairs.Add (pair);
			}

			return pairs;
		}, 1, -1);

		private static readonly IFunction	functionFilter = new NativeFunction ((values, scope, output) =>
		{
			List<Value>							arguments;
			IFunction							callback;
			List<KeyValuePair<Value, Value>>	result;

			callback = values[1].AsFunction;

			if (callback == null)
				return VoidValue.Instance;

			arguments = new List<Value> (values.Count - 1);
			result = new List<KeyValuePair<Value, Value>> (values[0].Fields.Count);

			foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
			{
				arguments.Clear ();
				arguments.Add (pair.Value);

				for (int i = 2; i < values.Count; ++i)
					arguments.Add (values[i]);

				if (callback.Execute (arguments, scope, output).AsBoolean)
					result.Add (new KeyValuePair<Value, Value> (pair.Key, pair.Value));
			}

			return result;
		});

		private static readonly IFunction	functionFind = new NativeFunction ((values) =>
		{
			int		index;
			int		offset;
			Value	search;
			Value	source;

			offset = values.Count > 2 ? (int)values[2].AsNumber : 0;
			search = values[1];
			source = values[0];

			if (source.Type == ValueContent.Map)
			{
				index = 0;

				foreach (KeyValuePair<Value, Value> pair in source.Fields)
				{
					if (++index > offset && pair.Value.Equals (search))
						return index - 1;
				}

				return -1;
			}
			else
				return source.AsString.IndexOf (search.AsString, offset, StringComparison.InvariantCulture);
		}, 2, 3);

		private static readonly IFunction	functionFlip = new NativeFunction ((values) =>
		{
			KeyValuePair<Value, Value>[]	flip;
			int								i;

			flip = new KeyValuePair<Value, Value>[values[0].Fields.Count];
			i = 0;

			foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
				flip[i++] = new KeyValuePair<Value, Value> (pair.Value, pair.Key);

			return flip;
		}, 1);

		private static readonly IFunction	functionFloor = new NativeFunction ((v) => Math.Floor (v[0].AsNumber), 1);

		private static readonly IFunction	functionFormat = new NativeFunction ((values) =>
		{
			CultureInfo	culture;
			string		format;
			int			index;
			object		target;

			culture = values.Count > 2 ? CultureInfo.GetCultureInfo (values[2].AsString) : CultureInfo.CurrentCulture;
			format = values[1].AsString;
			index = format.IndexOf (':');

			switch (index >= 0 ? format.Substring (0, index) : "a")
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
					target = epoch.AddSeconds ((double)values[0].AsNumber);

					break;

				case "dl":
					target = epoch.AddSeconds ((double)values[0].AsNumber).ToLocalTime ();

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

			return string.Format (culture, "{0:" + format.Substring (index + 1) + "}", target);
		}, 2, 3);

		private static readonly IFunction	functionHas = new NativeFunction ((values) =>
		{
			Value	source;

			source = values[0];

			for (int i = 1; i < values.Count; ++i)
				if (!source.Fields.Contains (values[i]))
					return false;

			return true;
		}, 1, -1);

		private static readonly IFunction	functionJoin = new NativeFunction ((values) =>
		{
			StringBuilder	builder;
			bool			first;
			string			split;

			if (values.Count > 1)
				split = values[1].AsString;
			else
				split = string.Empty;

			builder = new StringBuilder ();
			first = true;

			foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
			{
				if (first)
					first = false;
				else
					builder.Append (split);

				builder.Append (pair.Value.AsString);
			}

			return builder.ToString ();
		}, 1, 2);

		private static readonly IFunction	functionLength = new NativeFunction ((values) =>
		{
			if (values[0].Type == ValueContent.Map)
				return values[0].Fields.Count;

			return values[0].AsString.Length;
		}, 1);

		private static readonly IFunction	functionLowerCase = new NativeFunction ((v) => v[0].AsString.ToLowerInvariant (), 1);

		private static readonly IFunction	functionMap = new NativeFunction ((values, scope, output) =>
		{
			List<Value>						arguments;
			IFunction						callback;
			KeyValuePair<Value, Value>[]	result;
			int								i;

			callback = values[1].AsFunction;

			if (callback == null)
				return VoidValue.Instance;

			arguments = new List<Value> (values.Count - 1);
			i = 0;
			result = new KeyValuePair<Value, Value>[values[0].Fields.Count];

			foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
			{
				arguments.Clear ();
				arguments.Add (pair.Value);

				for (int j = 2; j < values.Count; ++j)
					arguments.Add (values[j]);

				result[i++] = new KeyValuePair<Value, Value> (pair.Key, callback.Execute (arguments, scope, output));
			}

			return result;
		}, 2, -1);

		private static readonly IFunction	functionMatch = new NativeFunction ((values) =>
		{
			List<Value>	groups;
			Match		match;

			match = Regex.Match (values[0].AsString, values[1].AsString);

			if (!match.Success)
				return VoidValue.Instance;

			groups = new List<Value> (match.Groups.Count);

			foreach (Group group in match.Groups)
				groups.Add (group.Value);

			return groups;
		}, 2, 3);

		private static readonly IFunction	functionMaximum = new NativeFunction ((values) =>
		{
			decimal max;

			max = values[0].AsNumber;

			for (int i = 1; i < values.Count; ++i)
				max = Math.Max (max, values[i].AsNumber);

			return max;
		}, 1, -1);

		private static readonly IFunction	functionMinimum = new NativeFunction ((values) =>
		{
			decimal min;

			min = values[0].AsNumber;

			for (int i = 1; i < values.Count; ++i)
				min = Math.Min (min, values[i].AsNumber);

			return min;
		}, 1, -1);

		private static readonly IFunction	functionOr = new NativeFunction ((values) =>
		{
			foreach (Value value in values)
			{
				if (value.AsBoolean)
					return true;
			}

			return false;
		});

		private static readonly IFunction	functionOrd = new NativeFunction ((values) =>
		{
			string	str;

			str = values[0].AsString;

			return str.Length > 0 ? char.ConvertToUtf32 (str, 0) : 0;
		}, 1);

		private static readonly IFunction	functionPower = new NativeFunction ((v) => Math.Pow ((double)v[0].AsNumber, (double)v[1].AsNumber), 2);

		private static readonly IFunction	functionRandom = new NativeFunction ((values) =>
		{
			lock (BuiltinFunctions.random)
			{
				switch (values.Count)
				{
					case 0:
						return BuiltinFunctions.random.Next ();
	
					case 1:
						return BuiltinFunctions.random.Next ((int)values[0].AsNumber);
	
					default:
						return BuiltinFunctions.random.Next ((int)values[0].AsNumber, (int)values[1].AsNumber);
				}
			}
		}, 0, 2);

		private static readonly IFunction	functionRange = new NativeFunction ((values) =>
		{
			int sign;
			int	start;
			int	step;
			int	stop;

			start = values.Count > 1 ? (int)values[0].AsNumber : 0;
			step = values.Count > 2 ? (int)values[2].AsNumber : 1;
			stop = values.Count > 1 ? (int)values[1].AsNumber : (int)values[0].AsNumber;

			if (step == 0)
				return MapValue.Empty;

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

			return new MapValue ((i) => start + step * i, (stop - start + step + sign) / step);
		}, 1, 3);

		private static readonly IFunction	functionRound = new NativeFunction ((values) =>
		{
			if (values.Count > 1)
				return Math.Round (values[0].AsNumber, (int)values[1].AsNumber);

			return Math.Round (values[0].AsNumber);
		}, 1, 2);

		private static readonly IFunction	functionSine = new NativeFunction ((v) => Math.Sin ((double)v[0].AsNumber), 1);

		private static readonly IFunction	functionSlice = new NativeFunction ((values) =>
		{
			int										count;
			IEnumerator<KeyValuePair<Value, Value>>	enumerator;
			int										length;
			int										offset;
			Value									source;
			KeyValuePair<Value, Value>[]			target;
			int										i;

			source = values[0];
			length = source.Type == ValueContent.Map ? source.Fields.Count : source.AsString.Length;
			offset = Math.Min ((int)values[1].AsNumber, length);
			count = values.Count > 2 ? Math.Min ((int)values[2].AsNumber, length - offset) : length - offset;

			if (source.Type == ValueContent.Map)
			{
				enumerator = source.Fields.GetEnumerator ();

				while (offset-- > 0 && enumerator.MoveNext ())
					;

				target = new KeyValuePair<Value, Value>[count];
				i = 0;

				while (count-- > 0 && enumerator.MoveNext ())
					target[i++] = enumerator.Current;

				return target;
			}

			return source.AsString.Substring (offset, count);
		}, 2, 3);

		private static readonly IFunction	functionSort = new NativeFunction ((values, scope, output) =>
		{
			IFunction							callback;
			List<KeyValuePair<Value, Value>>	sorted;

			callback = values.Count > 1 ? values[1].AsFunction : null;
			sorted = new List<KeyValuePair<Value, Value>> (values[0].Fields);

			if (callback != null)
				sorted.Sort ((a, b) => (int)callback.Execute (new Value[] {a.Value, b.Value}, scope, output).AsNumber);
			else
				sorted.Sort ((a, b) => a.Value.CompareTo (b.Value));

			return sorted;
		}, 1, 2);

		private static readonly IFunction	functionSplit = new NativeFunction ((v) => Array.ConvertAll (v[0].AsString.Split (new [] {v[1].AsString}, StringSplitOptions.None), s => new StringValue (s)), 2);

		private static readonly IFunction	functionToken = new NativeFunction ((values) =>
		{
			string	search;
			string	source;
			int		start;
			int		stop;

			search = values[1].AsString;
			source = values[0].AsString;
			start = 0;
			stop = source.IndexOf (search, StringComparison.InvariantCulture);

			for (int i = Math.Max ((int)values[2].AsNumber, 0); i > 0; --i)
			{
				if (stop == -1)
				{
					start = -1;

					break;
				}

				start = stop + search.Length;
				stop = source.IndexOf (search, start, StringComparison.InvariantCulture);
			}

			if (values.Count < 4)
			{
				if (start < 0)
					return string.Empty;
				else if (stop < 0)
					return source.Substring (start);
				else
					return source.Substring (start, stop - start);
			}
			else
			{
				if (start < 0)
					return source + search + values[3].AsString;
				else if (stop < 0)
					return source.Substring (0, start) + values[3].AsString;
				else
					return source.Substring (0, start) + values[3].AsString + source.Substring (stop);
			}
        }, 3, 4);

		private static readonly IFunction	functionUnion = new NativeFunction ((values) =>
		{
			Dictionary<Value, Value>	result;

			result = new Dictionary<Value, Value> ();

			foreach (Value value in values)
			{
				foreach (KeyValuePair<Value, Value> pair in value.Fields)
					result[pair.Key] = pair.Value;
			}

			return result;
		}, 0, -1);

		private static readonly IFunction	functionUpperCase = new NativeFunction ((v) => v[0].AsString.ToUpperInvariant (), 1);

		private static readonly IFunction	functionWhen = new NativeFunction ((values) =>
		{
			if (values[0].AsBoolean)
				return values[1];

			return values.Count > 2 ? values[2] : VoidValue.Instance;
		}, 2, 3);

		private static readonly IFunction	functionXor = new NativeFunction ((values) =>
		{
			int count;

			count = 0;

			foreach (Value value in values)
			{
				if (value.AsBoolean)
					++count;
			}

			return count == 1;
		});

		private static readonly IFunction	functionZip = new NativeFunction ((values) =>
		{
			IEnumerator<KeyValuePair<Value, Value>> enumerator1;
			IEnumerator<KeyValuePair<Value, Value>> enumerator2;
			IMap									map1;
			IMap									map2;
			List<KeyValuePair<Value, Value>>		result;

			map1 = values[0].Fields;
			map2 = values[1].Fields;

			enumerator1 = map1.GetEnumerator ();
			enumerator2 = map2.GetEnumerator ();
			result = new List<KeyValuePair<Value, Value>> (Math.Min (map1.Count, map2.Count));

			while (enumerator1.MoveNext () && enumerator2.MoveNext ())
				result.Add (new KeyValuePair<Value, Value> (enumerator1.Current.Value, enumerator2.Current.Value));

			return result;
		}, 2);

		private static readonly Dictionary<string, IFunction>	instances = new Dictionary<string, IFunction>
		{
			{"abs",		BuiltinFunctions.functionAbsolute},
			{"add",		BuiltinOperators.operatorAdd},
			{"and",		BuiltinFunctions.functionAnd},
			{"call",	BuiltinFunctions.functionCall},
			{"cast",	BuiltinFunctions.functionCast},
			{"cat",		BuiltinFunctions.functionCat},
			{"ceil",	BuiltinFunctions.functionCeiling},
			{"char",	BuiltinFunctions.functionChar},
			{"cmp",		BuiltinFunctions.functionCompare},
			{"cos",		BuiltinFunctions.functionCosine},
			{"cross",	BuiltinFunctions.functionCross},
			{"default",	BuiltinFunctions.functionDefault},
			{"div",		BuiltinOperators.operatorDiv},
			{"eq",		BuiltinOperators.operatorEqual},
			{"except",	BuiltinFunctions.functionExcept},
			{"filter",	BuiltinFunctions.functionFilter},
			{"find",	BuiltinFunctions.functionFind},
			{"flip",	BuiltinFunctions.functionFlip},
			{"floor",	BuiltinFunctions.functionFloor},
			{"format",	BuiltinFunctions.functionFormat},
			{"ge",		BuiltinOperators.operatorGreaterEqual},
			{"gt",		BuiltinOperators.operatorGreaterThan},
			{"has",		BuiltinFunctions.functionHas},
			{"join",	BuiltinFunctions.functionJoin},
			{"lcase",	BuiltinFunctions.functionLowerCase},
			{"le",		BuiltinOperators.operatorLowerEqual},
			{"len",		BuiltinFunctions.functionLength},
			{"lt",		BuiltinOperators.operatorLowerThan},
			{"map",		BuiltinFunctions.functionMap},
			{"match",	BuiltinFunctions.functionMatch},
			{"max",		BuiltinFunctions.functionMaximum},
			{"min",		BuiltinFunctions.functionMinimum},
			{"mod",		BuiltinOperators.operatorMod},
			{"mul",		BuiltinOperators.operatorMul},
			{"ne",		BuiltinOperators.operatorNotEqual},
			{"not",		BuiltinOperators.operatorNot},
			{"or",		BuiltinFunctions.functionOr},
			{"ord",		BuiltinFunctions.functionOrd},
			{"pow",		BuiltinFunctions.functionPower},
			{"rand",	BuiltinFunctions.functionRandom},
			{"range",	BuiltinFunctions.functionRange},
			{"round",	BuiltinFunctions.functionRound},
			{"sin",		BuiltinFunctions.functionSine},
			{"slice",	BuiltinFunctions.functionSlice},
			{"sort",	BuiltinFunctions.functionSort},
			{"split",	BuiltinFunctions.functionSplit},
			{"sub",		BuiltinOperators.operatorSub},
			{"token",	BuiltinFunctions.functionToken},
			{"ucase",	BuiltinFunctions.functionUpperCase},
			{"union",	BuiltinFunctions.functionUnion},
			{"when",	BuiltinFunctions.functionWhen},
			{"xor",		BuiltinFunctions.functionXor},
			{"zip",		BuiltinFunctions.functionZip}
		};

		private static readonly Random	random = new Random ();

		#endregion

		#region Methods

		public static bool TryGet (string name, out IFunction function)
		{
			return BuiltinFunctions.instances.TryGetValue (name, out function);
		}

		#endregion
	}
}
