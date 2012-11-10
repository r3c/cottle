using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Cottle.Functions;
using Cottle.Values;

namespace   Cottle.Commons
{
    public static class CommonFunctions
    {
        #region Properties
        
        public static bool  IncludeCacheEnable
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

        public static int   IncludeCacheSize
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

        private static readonly IFunction   functionAbsolute = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return Math.Abs (values[0].AsNumber);
        }, 1);

        private static readonly IFunction   functionAdd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber + values[1].AsNumber;
        }, 2);

        private static readonly IFunction   functionAnd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            foreach (Value value in values)
            {
                if (!value.AsBoolean)
                    return false;
            }

            return true;
        });

        private static readonly IFunction   functionCall = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Value[]     arguments;
            IFunction   function;
            int         i;

            function = values[0].AsFunction;

            if (function == null)
                return UndefinedValue.Instance;

            arguments = new Value[values[1].Fields.Count];
            i = 0;

            foreach (KeyValuePair<Value, Value> pair in values[1].Fields)
                arguments[i++] = pair.Value;

            return function.Execute(arguments, scope, output);
        }, 2);

        private static readonly IFunction   functionCast = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
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
                    return UndefinedValue.Instance;
            }
        }, 2);

        private static readonly IFunction   functionCat = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            StringBuilder                       builder;
            List<KeyValuePair<Value, Value>>    list;

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

        private static readonly IFunction   functionChar = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
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

        private static readonly IFunction   functionCompare = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].CompareTo (values[1]);
        }, 2);

        private static readonly IFunction   functionCross = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            bool                                insert;
            List<KeyValuePair<Value, Value>>    pairs;

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

        private static readonly IFunction   functionDefault = new CallbackFunction(delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsBoolean ? values[0] : values[1];
        }, 2);

        private static readonly IFunction   functionDiv = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal denominator = values[1].AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return values[0].AsNumber / denominator;
        }, 2);

        private static readonly IFunction   functionEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Value   first = values[0];

            for (int i = 1; i < values.Count; ++i)
                if (values[i].CompareTo (first) != 0)
                    return false;

            return true;
        }, 1, -1);

        private static readonly IFunction   functionExcept = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            bool                                insert;
            List<KeyValuePair<Value, Value>>    pairs;

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

        private static readonly IFunction   functionFilter = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            List<Value>                         arguments = new List<Value> (values.Count - 1);
            IFunction                           callback = values[1].AsFunction;
            List<KeyValuePair<Value, Value>>    result = new List<KeyValuePair<Value, Value>> (values[0].Fields.Count);

            if (callback == null)
                return UndefinedValue.Instance;

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

        private static readonly IFunction   functionFind = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int     offset = values.Count > 2 ? (int)values[2].AsNumber : 0;
            Value   search = values[1];
            Value   value = values[0];
            int     i;

            if (value.Type == ValueContent.Map)
            {
                i = 0;

                foreach (KeyValuePair<Value, Value> pair in value.Fields)
                {
                    if (++i > offset && pair.Value.Equals (search))
                        return i - 1;
                }

                return -1;
            }
            else
                return value.AsString.IndexOf (search.AsString, offset, StringComparison.InvariantCulture);
        }, 2, 3);

        private static readonly IFunction   functionFlip = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            KeyValuePair<Value, Value>[]    flip;
            int                             i;

            flip = new KeyValuePair<Value, Value>[values[0].Fields.Count];
            i = 0;

            foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
                flip[i++] = new KeyValuePair<Value, Value> (pair.Value, pair.Key);

            return flip;
        }, 1);

        private static readonly IFunction   functionFormat = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            CultureInfo culture;
            string      format;
            int         index;
            object      target;

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
                    return UndefinedValue.Instance;
            }

            return string.Format (culture, "{0:" + format.Substring(index + 1) + "}", target);
        }, 2, 3);

        private static readonly IFunction   functionGreater = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].CompareTo (values[1]) > 0;
        }, 2);

        private static readonly IFunction   functionGreaterEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].CompareTo (values[1]) >= 0;
        }, 2);

        private static readonly IFunction   functionHas = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Value   value = values[0];

            for (int i = 1; i < values.Count; ++i)
                if (!value.Fields.Contains (values[i]))
                    return false;

            return true;
        }, 1, -1);

        private static readonly IFunction   functionInclude = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Document    document;
            object      entry;
            Scope       inner;
            string      path;
            DateTime    write;

            path = Path.GetFullPath (values[0].AsString);

            if (!File.Exists (path))
                return UndefinedValue.Instance;

            write = File.GetLastWriteTime (path);

            lock (CommonFunctions.includeCache)
            {
                if (CommonFunctions.includeCacheEnable)
                    entry = CommonFunctions.includeCache[path];
                else
                    entry = null;

                if (entry != null && ((KeyValuePair<Document, DateTime>)entry).Value >= write)
                    document = ((KeyValuePair<Document, DateTime>)entry).Key;
                else
                {
                    using (FileStream stream = File.OpenRead (path))
                    {
                        document = new Document (new StreamReader (stream));
                    }

                    CommonFunctions.includeCache[path] = new KeyValuePair<Document, DateTime> (document, write);

                    while (CommonFunctions.includeCache.Count > CommonFunctions.includeCacheSize && CommonFunctions.includeCache.Count > 0)
                        CommonFunctions.includeCache.RemoveAt (0);
                }
            }

            inner = new Scope ();

            for (int i = 1; i < values.Count; ++i)
            {
                foreach (KeyValuePair<Value, Value> pair in values[i].Fields)
                    inner.Set (pair.Key, pair.Value, ScopeMode.Closest);
            }

            return document.Render (inner, output);
        }, 1, -1);

        private static readonly IFunction   functionJoin = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            StringBuilder   builder = new StringBuilder ();
            bool            first = true;
            string          sep;

            if (values.Count > 1)
                sep = values[1].AsString;
            else
                sep = string.Empty;

            foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
            {
                if (first)
                    first = false;
                else
                    builder.Append (sep);

                builder.Append (pair.Value.AsString);
            }

            return builder.ToString ();
        }, 1, 2);

        private static readonly IFunction   functionLength = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            if (values[0].Type == ValueContent.Map)
                return values[0].Fields.Count;

            return values[0].AsString.Length;
        }, 1);

        private static readonly IFunction   functionLower = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].CompareTo (values[1]) < 0;
        }, 2);

        private static readonly IFunction   functionLowerCase = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsString.ToLowerInvariant ();
        }, 1);

        private static readonly IFunction   functionLowerEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].CompareTo (values[1]) <= 0;
        }, 2);

        private static readonly IFunction   functionMap = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            List<Value>                     arguments = new List<Value> (values.Count - 1);
            IFunction                       callback = values[1].AsFunction;
            KeyValuePair<Value, Value>[]    result = new KeyValuePair<Value, Value>[values[0].Fields.Count];
            int                             i = 0;

            if (callback == null)
                return UndefinedValue.Instance;

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

        private static readonly IFunction   functionMatch = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            List<Value> groups;
            Match       match;

            match = Regex.Match (values[0].AsString, values[1].AsString);

            if (!match.Success)
                return UndefinedValue.Instance;

            groups = new List<Value> (match.Groups.Count);

            foreach (Group group in match.Groups)
                groups.Add (group.Value);

            return groups;
        }, 2, 3);

        private static readonly IFunction   functionMaximum = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal max;

            max = values[0].AsNumber;

            for (int i = 1; i < values.Count; ++i)
                max = Math.Max (max, values[i].AsNumber);

            return max;
        }, 1, -1);

        private static readonly IFunction   functionMinimum = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal min;

            min = values[0].AsNumber;

            for (int i = 1; i < values.Count; ++i)
                min = Math.Min (min, values[i].AsNumber);

            return min;
        }, 1, -1);

        private static readonly IFunction   functionMod = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal denominator = values[1].AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return values[0].AsNumber % denominator;
        }, 2);

        private static readonly IFunction   functionMul = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber * values[1].AsNumber;
        }, 2);

        private static readonly IFunction   functionNot = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return !values[0].AsBoolean;
        }, 1);

        private static readonly IFunction   functionOr = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            foreach (Value value in values)
            {
                if (value.AsBoolean)
                    return true;
            }

            return false;
        });

        private static readonly IFunction   functionOrd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            string  str = values[0].AsString;

            return str.Length > 0 ? char.ConvertToUtf32 (str, 0) : 0;
        }, 1);

        private static readonly IFunction   functionRandom = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
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

        private static readonly IFunction   functionRange = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            List<Value> array;
            int         start;
            int         step;
            int         stop;

            start = values.Count > 1 ? (int)values[0].AsNumber : 0;
            step = values.Count > 2 ? (int)values[2].AsNumber : 1;
            stop = values.Count > 1 ? (int)values[1].AsNumber : (int)values[0].AsNumber;

            if (step == 0 || (start < stop && step < 0) || (start > stop && step > 0))
                return UndefinedValue.Instance;

            array = new List<Value> ();

            for (; start < stop; start += step)
                array.Add (start);

            return array;
        }, 1, 3);

        private static readonly IFunction   functionSlice = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int                                     count;
            IEnumerator<KeyValuePair<Value, Value>> enumerator;
            int                                     length;
            int                                     offset;
            Value                                   source;
            KeyValuePair<Value, Value>[]            target;
            int                                     i;

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

        private static readonly IFunction   functionSort = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            IFunction                           callback = values.Count > 1 ? values[1].AsFunction : null;
            List<KeyValuePair<Value, Value>>    sorted = new List<KeyValuePair<Value, Value>> (values[0].Fields);

            if (callback != null)
                sorted.Sort ((a, b) => (int)callback.Execute (new Value[] {a.Value, b.Value}, scope, output).AsNumber);
            else
                sorted.Sort ((a, b) => a.Value.CompareTo (b.Value));

            return sorted;
        }, 1, 2);

        private static readonly IFunction   functionSplit = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return Array.ConvertAll (values[0].AsString.Split (new string[] {values[1].AsString}, StringSplitOptions.None), s => new StringValue (s));
        }, 2);

        private static readonly IFunction   functionSub = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber - values[1].AsNumber;
        }, 2);

        private static readonly IFunction   functionUnion = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Dictionary<Value, Value>    result = new Dictionary<Value, Value> ();

            foreach (Value value in values)
            {
                foreach (KeyValuePair<Value, Value> pair in value.Fields)
                    result[pair.Key] = pair.Value;
            }

            return result;
        }, 0, -1);

        private static readonly IFunction   functionUpperCase = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsString.ToUpperInvariant ();
        }, 1);

        private static readonly IFunction   functionWhen = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            if (values[0].AsBoolean)
                return values[1];

            return values.Count > 2 ? values[2] : UndefinedValue.Instance;
        }, 2, 3);

        private static readonly IFunction   functionXor = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int count = 0;

            foreach (Value value in values)
            {
                if (value.AsBoolean)
                    ++count;
            }

            return count == 1;
        });

        private static readonly IFunction   functionZip = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            IEnumerator<KeyValuePair<Value, Value>> enumerator1 = values[0].Fields.GetEnumerator ();
            IEnumerator<KeyValuePair<Value, Value>> enumerator2 = values[1].Fields.GetEnumerator ();
            List<KeyValuePair<Value, Value>>        pairs = new List<KeyValuePair<Value, Value>> ();

            while (enumerator1.MoveNext () && enumerator2.MoveNext ())
                pairs.Add (new KeyValuePair<Value, Value> (enumerator1.Current.Value, enumerator2.Current.Value));

            return pairs;
        }, 2);

        #endregion

        #region Attributes / Static

        private static readonly DateTime            epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static readonly OrderedDictionary   includeCache = new OrderedDictionary ();
        
        private static bool                         includeCacheEnable = true;

        private static int                          includeCacheSize = 256;

        [ThreadStatic]
        private static Random                       random = null;

        #endregion

        #region Methods

        public static void  Assign (Scope scope, ScopeMode mode = ScopeMode.Closest)
        {
            scope.Set ("abs", new FunctionValue (CommonFunctions.functionAbsolute), mode);
            scope.Set ("add", new FunctionValue (CommonFunctions.functionAdd), mode);
            scope.Set ("and", new FunctionValue (CommonFunctions.functionAnd), mode);
            scope.Set ("call", new FunctionValue (CommonFunctions.functionCall), mode);
            scope.Set ("cast", new FunctionValue (CommonFunctions.functionCast), mode);
            scope.Set ("cat", new FunctionValue (CommonFunctions.functionCat), mode);
            scope.Set ("char", new FunctionValue (CommonFunctions.functionChar), mode);
            scope.Set ("cmp", new FunctionValue (CommonFunctions.functionCompare), mode);
            scope.Set ("cross", new FunctionValue (CommonFunctions.functionCross), mode);
            scope.Set ("default", new FunctionValue (CommonFunctions.functionDefault), mode);
            scope.Set ("div", new FunctionValue (CommonFunctions.functionDiv), mode);
            scope.Set ("eq", new FunctionValue (CommonFunctions.functionEqual), mode);
            scope.Set ("except", new FunctionValue (CommonFunctions.functionExcept), mode);
            scope.Set ("filter", new FunctionValue (CommonFunctions.functionFilter), mode);
            scope.Set ("find", new FunctionValue (CommonFunctions.functionFind), mode);
            scope.Set ("flip", new FunctionValue (CommonFunctions.functionFlip), mode);
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
            scope.Set ("rand", new FunctionValue (CommonFunctions.functionRandom), mode);
            scope.Set ("range", new FunctionValue (CommonFunctions.functionRange), mode);
            scope.Set ("slice", new FunctionValue (CommonFunctions.functionSlice), mode);
            scope.Set ("sort", new FunctionValue (CommonFunctions.functionSort), mode);
            scope.Set ("split", new FunctionValue (CommonFunctions.functionSplit), mode);
            scope.Set ("sub", new FunctionValue (CommonFunctions.functionSub), mode);
            scope.Set ("ucase", new FunctionValue (CommonFunctions.functionUpperCase), mode);
            scope.Set ("union", new FunctionValue (CommonFunctions.functionUnion), mode);
            scope.Set ("when", new FunctionValue (CommonFunctions.functionWhen), mode);
            scope.Set ("xor", new FunctionValue (CommonFunctions.functionXor), mode);
            scope.Set ("zip", new FunctionValue (CommonFunctions.functionZip), mode);
        }

        #endregion
    }
}
