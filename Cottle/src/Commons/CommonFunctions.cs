using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Activation;
using System.Text;
using System.Text.RegularExpressions;

using Cottle.Documents;
using Cottle.Functions;
using Cottle.Scopes;
using Cottle.Values;

namespace Cottle.Commons
{
	public static class CommonFunctions
	{
		#region Properties

		public static bool	IncludeCacheEnable
		{
			get
			{
				return CommonFunctions.includeCacheEnable;
			}
			set
			{
				CommonFunctions.includeCacheEnable = value;
			}
		}

		public static int	IncludeCacheSize
		{
			get
			{
				return CommonFunctions.includeCacheSize;
			}
			set
			{
				CommonFunctions.includeCacheSize = value;
			}
		}

		#endregion

		#region Attributes / Instance

		private static readonly IFunction	functionAbsolute = new CallbackFunction ((values, scope, output) =>
		{
			return Math.Abs (values[0].AsNumber);
		}, 1);

		private static readonly IFunction	functionAdd = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].AsNumber + values[1].AsNumber;
		}, 2);

		private static readonly IFunction	functionAnd = new CallbackFunction ((values, scope, output) =>
		{
			foreach (Value value in values)
			{
				if (!value.AsBoolean)
					return false;
			}

			return true;
		});

		private static readonly IFunction	functionCall = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionCast = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionCat = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionCeiling = new CallbackFunction ((values, scope, output) =>
		{
			return Math.Ceiling (values[0].AsNumber);
		}, 1);

		private static readonly IFunction	functionChar = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionCompare = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].CompareTo (values[1]);
		}, 2);

		private static readonly IFunction	functionCosine = new CallbackFunction ((values, scope, output) =>
		{
			return Math.Cos ((double)values[0].AsNumber);
		}, 1);

		private static readonly IFunction	functionCross = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionDefault = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].AsBoolean ? values[0] : values[1];
		}, 2);

		private static readonly IFunction	functionDiv = new CallbackFunction ((values, scope, output) =>
		{
			decimal denominator;

			denominator = values[1].AsNumber;

			if (denominator == 0)
				return VoidValue.Instance;

			return values[0].AsNumber / denominator;
		}, 2);

		private static readonly IFunction	functionEqual = new CallbackFunction ((values, scope, output) =>
		{
			Value	first;

			first = values[0];

			for (int i = 1; i < values.Count; ++i)
				if (values[i].CompareTo (first) != 0)
					return false;

			return true;
		}, 1, -1);

		private static readonly IFunction	functionExcept = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionFilter = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionFind = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionFlip = new CallbackFunction ((values, scope, output) =>
		{
			KeyValuePair<Value, Value>[]	flip;
			int								i;

			flip = new KeyValuePair<Value, Value>[values[0].Fields.Count];
			i = 0;

			foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
				flip[i++] = new KeyValuePair<Value, Value> (pair.Value, pair.Key);

			return flip;
		}, 1);

		private static readonly IFunction	functionFloor = new CallbackFunction ((values, scope, output) =>
		{
			return Math.Floor (values[0].AsNumber);
		}, 1);

		private static readonly IFunction	functionFormat = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionGreater = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].CompareTo (values[1]) > 0;
		}, 2);

		private static readonly IFunction	functionGreaterEqual = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].CompareTo (values[1]) >= 0;
		}, 2);

		private static readonly IFunction	functionHas = new CallbackFunction ((values, scope, output) =>
		{
			Value	source;

			source = values[0];

			for (int i = 1; i < values.Count; ++i)
				if (!source.Fields.Contains (values[i]))
					return false;

			return true;
		}, 1, -1);

		private static readonly IFunction	functionInclude = new CallbackFunction ((values, scope, output) =>
		{
			IDocument	document;
			object		entry;
			IScope		inner;
			string		path;
			DateTime	write;

			path = Path.GetFullPath (values[0].AsString);

			if (!File.Exists (path))
				return VoidValue.Instance;

			write = File.GetLastWriteTime (path);

			lock (CommonFunctions.includeCache)
			{
				if (CommonFunctions.includeCacheEnable)
					entry = CommonFunctions.includeCache[path];
				else
					entry = null;

				if (entry != null && ((KeyValuePair<IDocument, DateTime>)entry).Value >= write)
					document = ((KeyValuePair<IDocument, DateTime>)entry).Key;
				else
				{
					using (FileStream stream = File.OpenRead (path))
					{
						document = new SimpleDocument (new StreamReader (stream));
					}

					CommonFunctions.includeCache[path] = new KeyValuePair<IDocument, DateTime> (document, write);

					while (CommonFunctions.includeCache.Count > CommonFunctions.includeCacheSize && CommonFunctions.includeCache.Count > 0)
						CommonFunctions.includeCache.RemoveAt (0);
				}
			}

			inner = new FallbackScope (scope, new SimpleScope ());

			for (int i = 1; i < values.Count; ++i)
			{
				foreach (KeyValuePair<Value, Value> pair in values[i].Fields)
					inner.Set (pair.Key, pair.Value, ScopeMode.Closest);
			}

			return document.Render (inner, output);
		}, 1, -1);

		private static readonly IFunction	functionJoin = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionLength = new CallbackFunction ((values, scope, output) =>
		{
			if (values[0].Type == ValueContent.Map)
				return values[0].Fields.Count;

			return values[0].AsString.Length;
		}, 1);

		private static readonly IFunction	functionLower = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].CompareTo (values[1]) < 0;
		}, 2);

		private static readonly IFunction	functionLowerCase = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].AsString.ToLowerInvariant ();
		}, 1);

		private static readonly IFunction	functionLowerEqual = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].CompareTo (values[1]) <= 0;
		}, 2);

		private static readonly IFunction	functionMap = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionMatch = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionMaximum = new CallbackFunction ((values, scope, output) =>
		{
			decimal max;

			max = values[0].AsNumber;

			for (int i = 1; i < values.Count; ++i)
				max = Math.Max (max, values[i].AsNumber);

			return max;
		}, 1, -1);

		private static readonly IFunction	functionMinimum = new CallbackFunction ((values, scope, output) =>
		{
			decimal min;

			min = values[0].AsNumber;

			for (int i = 1; i < values.Count; ++i)
				min = Math.Min (min, values[i].AsNumber);

			return min;
		}, 1, -1);

		private static readonly IFunction	functionMod = new CallbackFunction ((values, scope, output) =>
		{
			decimal denominator;

			denominator = values[1].AsNumber;

			if (denominator == 0)
				return VoidValue.Instance;

			return values[0].AsNumber % denominator;
		}, 2);

		private static readonly IFunction	functionMul = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].AsNumber * values[1].AsNumber;
		}, 2);

		private static readonly IFunction	functionNot = new CallbackFunction ((values, scope, output) =>
		{
			return !values[0].AsBoolean;
		}, 1);

		private static readonly IFunction	functionOr = new CallbackFunction ((values, scope, output) =>
		{
			foreach (Value value in values)
			{
				if (value.AsBoolean)
					return true;
			}

			return false;
		});

		private static readonly IFunction	functionOrd = new CallbackFunction ((values, scope, output) =>
		{
			string	str;

			str = values[0].AsString;

			return str.Length > 0 ? char.ConvertToUtf32 (str, 0) : 0;
		}, 1);

		private static readonly IFunction	functionPower = new CallbackFunction ((values, scope, output) =>
		{
			return Math.Pow ((double)values[0].AsNumber, (double)values[1].AsNumber);
		}, 2);

		private static readonly IFunction	functionRandom = new CallbackFunction ((values, scope, output) =>
		{
			if (CommonFunctions.random == null)
				CommonFunctions.random = new Random ();

			switch (values.Count)
			{
				case 0:
					return CommonFunctions.random.Next ();

				case 1:
					return CommonFunctions.random.Next ((int)values[0].AsNumber);

				default:
					return CommonFunctions.random.Next ((int)values[0].AsNumber, (int)values[1].AsNumber);
			}
		}, 0, 2);

		private static readonly IFunction	functionRange = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionRound = new CallbackFunction ((values, scope, output) =>
		{
			if (values.Count > 1)
				return Math.Round (values[0].AsNumber, (int)values[1].AsNumber);

			return Math.Round (values[0].AsNumber);
		}, 1, 2);

		private static readonly IFunction	functionSine = new CallbackFunction ((values, scope, output) =>
		{
			return Math.Sin ((double)values[0].AsNumber);
		}, 1);

		private static readonly IFunction	functionSlice = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionSort = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionSplit = new CallbackFunction ((values, scope, output) =>
		{
			return Array.ConvertAll (values[0].AsString.Split (new [] {values[1].AsString}, StringSplitOptions.None), s => new StringValue (s));
		}, 2);

		private static readonly IFunction	functionSub = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].AsNumber - values[1].AsNumber;
		}, 2);

		private static readonly IFunction	functionToken = new CallbackFunction ((values, scope, output) =>
		{
			string	search;
			string	source;
			int		start;
			int		stop;

			search = values[1].AsString;
			source = values[0].AsString;
			start = 0;
			stop = source.IndexOf (search);

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

		private static readonly IFunction	functionUnion = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionUpperCase = new CallbackFunction ((values, scope, output) =>
		{
			return values[0].AsString.ToUpperInvariant ();
		}, 1);

		private static readonly IFunction	functionWhen = new CallbackFunction ((values, scope, output) =>
		{
			if (values[0].AsBoolean)
				return values[1];

			return values.Count > 2 ? values[2] : VoidValue.Instance;
		}, 2, 3);

		private static readonly IFunction	functionXor = new CallbackFunction ((values, scope, output) =>
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

		private static readonly IFunction	functionZip = new CallbackFunction ((values, scope, output) =>
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

		#endregion

		#region Attributes / Static

		private static readonly DateTime			epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static readonly OrderedDictionary	includeCache = new OrderedDictionary ();
		
		private static bool							includeCacheEnable = true;

		private static int							includeCacheSize = 256;

		[ThreadStatic]
		private static Random						random = null;

		#endregion

		#region Methods

		public static void	Assign (IScope scope, ScopeMode mode = ScopeMode.Closest)
		{
			scope.Set ("abs", new FunctionValue (CommonFunctions.functionAbsolute), mode);
			scope.Set ("add", new FunctionValue (CommonFunctions.functionAdd), mode);
			scope.Set ("and", new FunctionValue (CommonFunctions.functionAnd), mode);
			scope.Set ("call", new FunctionValue (CommonFunctions.functionCall), mode);
			scope.Set ("cast", new FunctionValue (CommonFunctions.functionCast), mode);
			scope.Set ("cat", new FunctionValue (CommonFunctions.functionCat), mode);
			scope.Set ("ceil", new FunctionValue (CommonFunctions.functionCeiling), mode);
			scope.Set ("char", new FunctionValue (CommonFunctions.functionChar), mode);
			scope.Set ("cmp", new FunctionValue (CommonFunctions.functionCompare), mode);
			scope.Set ("cos", new FunctionValue (CommonFunctions.functionCosine), mode);
			scope.Set ("cross", new FunctionValue (CommonFunctions.functionCross), mode);
			scope.Set ("default", new FunctionValue (CommonFunctions.functionDefault), mode);
			scope.Set ("div", new FunctionValue (CommonFunctions.functionDiv), mode);
			scope.Set ("eq", new FunctionValue (CommonFunctions.functionEqual), mode);
			scope.Set ("except", new FunctionValue (CommonFunctions.functionExcept), mode);
			scope.Set ("filter", new FunctionValue (CommonFunctions.functionFilter), mode);
			scope.Set ("find", new FunctionValue (CommonFunctions.functionFind), mode);
			scope.Set ("flip", new FunctionValue (CommonFunctions.functionFlip), mode);
			scope.Set ("floor", new FunctionValue (CommonFunctions.functionFloor), mode);
			scope.Set ("format", new FunctionValue (CommonFunctions.functionFormat), mode);
			scope.Set ("ge", new FunctionValue (CommonFunctions.functionGreaterEqual), mode);
			scope.Set ("gt", new FunctionValue (CommonFunctions.functionGreater), mode);
			scope.Set ("has", new FunctionValue (CommonFunctions.functionHas), mode);
			scope.Set ("include", new FunctionValue (CommonFunctions.functionInclude), mode);
			scope.Set ("join", new FunctionValue (CommonFunctions.functionJoin), mode);
			scope.Set ("lcase", new FunctionValue (CommonFunctions.functionLowerCase), mode);
			scope.Set ("le", new FunctionValue (CommonFunctions.functionLowerEqual), mode);
			scope.Set ("len", new FunctionValue (CommonFunctions.functionLength), mode);
			scope.Set ("lt", new FunctionValue (CommonFunctions.functionLower), mode);
			scope.Set ("map", new FunctionValue (CommonFunctions.functionMap), mode);
			scope.Set ("match", new FunctionValue (CommonFunctions.functionMatch), mode);
			scope.Set ("max", new FunctionValue (CommonFunctions.functionMaximum), mode);
			scope.Set ("min", new FunctionValue (CommonFunctions.functionMinimum), mode);
			scope.Set ("mod", new FunctionValue (CommonFunctions.functionMod), mode);
			scope.Set ("mul", new FunctionValue (CommonFunctions.functionMul), mode);
			scope.Set ("not", new FunctionValue (CommonFunctions.functionNot), mode);
			scope.Set ("or", new FunctionValue (CommonFunctions.functionOr), mode);
			scope.Set ("ord", new FunctionValue (CommonFunctions.functionOrd), mode);
			scope.Set ("pow", new FunctionValue (CommonFunctions.functionPower), mode);
			scope.Set ("rand", new FunctionValue (CommonFunctions.functionRandom), mode);
			scope.Set ("range", new FunctionValue (CommonFunctions.functionRange), mode);
			scope.Set ("round", new FunctionValue (CommonFunctions.functionRound), mode);
			scope.Set ("sin", new FunctionValue (CommonFunctions.functionSine), mode);
			scope.Set ("slice", new FunctionValue (CommonFunctions.functionSlice), mode);
			scope.Set ("sort", new FunctionValue (CommonFunctions.functionSort), mode);
			scope.Set ("split", new FunctionValue (CommonFunctions.functionSplit), mode);
			scope.Set ("sub", new FunctionValue (CommonFunctions.functionSub), mode);
			scope.Set ("token", new FunctionValue (CommonFunctions.functionToken), mode);
			scope.Set ("ucase", new FunctionValue (CommonFunctions.functionUpperCase), mode);
			scope.Set ("union", new FunctionValue (CommonFunctions.functionUnion), mode);
			scope.Set ("when", new FunctionValue (CommonFunctions.functionWhen), mode);
			scope.Set ("xor", new FunctionValue (CommonFunctions.functionXor), mode);
			scope.Set ("zip", new FunctionValue (CommonFunctions.functionZip), mode);
		}

		#endregion
	}
}
