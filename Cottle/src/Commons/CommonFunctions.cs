using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Cottle.Functions;
using Cottle.Values;

namespace   Cottle.Commons
{
    public static class CommonFunctions
    {
        #region Constants

        public static readonly IFunction    FunctionAbsolute = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return Math.Abs (values[0].AsNumber);
        }, 1);

        public static readonly IFunction    FunctionAdd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber + values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionAnd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            foreach (Value value in values)
            {
                if (!value.AsBoolean)
                    return false;
            }

            return true;
        });

        public static readonly IFunction    FunctionCat = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            List<KeyValuePair<Value, Value>>    array;
            StringBuilder                       builder;

            if (values[0].Type == Value.DataType.ARRAY)
            {
                array = new List<KeyValuePair<Value, Value>> (values[0].Fields.Count * 2 + 1);

                foreach (Value value in values)
                    array.AddRange (value.Fields);

                return array;
            }
            else
            {
                builder = new StringBuilder ();

                foreach (Value value in values)
                    builder.Append (value.AsString);

                return builder.ToString ();
            }
        }, 1, -1);

        public static readonly IFunction    FunctionChar = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
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

        public static readonly IFunction    FunctionCompare = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].CompareTo (values[1]);
        }, 2);

        public static readonly IFunction    FunctionCross = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int                         i;
            bool                        insert;
            Dictionary<Value, Value>    result = new Dictionary<Value, Value> ();

            foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
            {
                insert = true;

                for (i = 1; i < values.Count; ++i)
                {
                    if (!values[i].Has (pair.Key))
                    {
                        insert = false;

                        break;
                    }
                }

                if (insert)
                    result[pair.Key] = pair.Value;
            }

            return result;
        }, 1, -1);

        public static readonly IFunction    FunctionDiv = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal denominator = values[1].AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return values[0].AsNumber / denominator;
        }, 2);

        public static readonly IFunction    FunctionEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal compare = values[0].AsNumber;
            int     i;

            for (i = 1; i < values.Count; ++i)
                if (values[i].AsNumber != compare)
                    return false;

            return true;
        }, 1, -1);

        public static readonly IFunction    FunctionExcept = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int                         i;
            bool                        insert;
            Dictionary<Value, Value>    result = new Dictionary<Value, Value> ();

            foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
            {
                insert = true;

                for (i = 1; i < values.Count; ++i)
                {
                    if (values[i].Has (pair.Key))
                    {
                        insert = false;

                        break;
                    }
                }

                if (insert)
                    result[pair.Key] = pair.Value;
            }

            return result;
        }, 1, -1);

        public static readonly IFunction    FunctionFind = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int     index = values.Count > 2 ? (int)values[2].AsNumber : 0;
            Value   token = values[1];
            Value   value = values[0];

            if (value.Type == Value.DataType.ARRAY)
                return value.Fields.FindIndex (index, p => p.Value.CompareTo (token) == 0);
            else
                return value.AsString.IndexOf (token.AsString, index);
        }, 2, 3);

        public static readonly IFunction    FunctionFlip = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].Fields.ConvertAll (p => new KeyValuePair<Value, Value> (p.Value, p.Key));
        }, 1);

        public static readonly IFunction    FunctionGreater = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber > values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionGreaterEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber >= values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionHas = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Value   value = values[0];
            int     i;

            for (i = 1; i < values.Count; ++i)
                if (!value.Has (values[i]))
                    return false;

            return true;
        }, 1, -1);

        public static readonly IFunction    FunctionInclude = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            KeyValuePair<Document, DateTime>    compiled;
            Scope                               inner;
            DateTime                            modified;
            string                              path;
            FileStream                          stream;
            int                                 i;

            path = Path.GetFullPath (values[0].AsString);

            try
            {
                modified = File.GetLastWriteTime (path);

                lock (CommonFunctions.includes)
                {
                    if (!CommonFunctions.includes.TryGetValue (path, out compiled) || compiled.Value < modified)
                    {
                        using (stream = File.OpenRead (path))
                        {
                            compiled = new KeyValuePair<Document, DateTime> (new Document (new StreamReader (stream)), modified);
                        }

                        CommonFunctions.includes[path] = compiled;
                    }
                }

                inner = new Scope();

                for (i = 1; i < values.Count; ++i)
                {
                    foreach (KeyValuePair<Value, Value> pair in values[i].Fields)
                        inner.Set(pair.Key, pair.Value, Scope.SetMode.ANYWHERE);
                }

                return compiled.Key.Render (inner, output);
            }
            catch
            {
                return UndefinedValue.Instance;
            }
        }, 1, -1);

        public static readonly IFunction    FunctionJoin = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
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

        public static readonly IFunction    FunctionLength = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            if (values[0].Type == Value.DataType.ARRAY)
                return values[0].Fields.Count;

            return values[0].AsString.Length;
        }, 1);

        public static readonly IFunction    FunctionLower = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber < values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionLowerCase = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsString.ToLowerInvariant ();
        }, 1);

        public static readonly IFunction    FunctionLowerEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber <= values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionMap = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            List<Value>                     arguments = new List<Value> (values.Count - 1);
            KeyValuePair<Value, Value>[]    array = new KeyValuePair<Value, Value>[values[0].Fields.Count];
            IFunction                       callback = values[1].AsFunction;
            int                             i = 0;
            int                             j;

            if (callback == null)
                return UndefinedValue.Instance;

            foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
            {
                arguments.Clear ();
                arguments.Add (pair.Value);

                for (j = 2; j < values.Count; ++j)
                    arguments.Add (values[j]);

                array[i++] = new KeyValuePair<Value, Value> (pair.Key, callback.Execute (arguments, scope, output));
            }

            return array;
        }, 2, -1);

        public static readonly IFunction    FunctionMatch = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            List<Value> groups;
            Match       match;

            try
            {
                match = Regex.Match (values[0].AsString, values[1].AsString);
            }
            catch
            {
                return UndefinedValue.Instance;
            }

            if (!match.Success)
                return UndefinedValue.Instance;

            groups = new List<Value> (match.Groups.Count);

            foreach (Group group in match.Groups)
                groups.Add (group.Value);

            return groups;
        }, 2, 3);

        public static readonly IFunction    FunctionMaximum = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Value   array;
            decimal max;
            int     i;

            if (values[0].Type == Value.DataType.ARRAY)
            {
                array = values[0];

                if (array.Fields.Count <= 0)
                    return UndefinedValue.Instance;

                max = array.Fields[0].Value.AsNumber;

                for (i = 1; i < array.Fields.Count; ++i)
                    max = Math.Max (max, array.Fields[i].Value.AsNumber);
            }
            else
            {
                max = values[0].AsNumber;

                for (i = 1; i < values.Count; ++i)
                    max = Math.Max (max, values[i].AsNumber);
            }

            return max;
        }, 1, -1);

        public static readonly IFunction    FunctionMinimum = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Value   array;
            decimal min;
            int     i;

            if (values[0].Type == Value.DataType.ARRAY)
            {
                array = values[0];

                if (array.Fields.Count <= 0)
                    return UndefinedValue.Instance;

                min = array.Fields[0].Value.AsNumber;

                for (i = 1; i < array.Fields.Count; ++i)
                    min = Math.Min (min, array.Fields[i].Value.AsNumber);
            }
            else
            {
                min = values[0].AsNumber;

                for (i = 1; i < values.Count; ++i)
                    min = Math.Min (min, values[i].AsNumber);
            }

            return min;
        }, 1, -1);

        public static readonly IFunction    FunctionMod = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal denominator = values[1].AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return values[0].AsNumber % denominator;
        }, 2);

        public static readonly IFunction    FunctionMul = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber * values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionNot = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return !values[0].AsBoolean;
        }, 1);

        public static readonly IFunction    FunctionOr = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            foreach (Value value in values)
            {
                if (value.AsBoolean)
                    return true;
            }

            return false;
        });

        public static readonly IFunction    FunctionOrd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            string  str = values[0].AsString;

            return str.Length > 0 ? char.ConvertToUtf32 (str, 0) : 0;
        }, 1);

        public static readonly IFunction    FunctionRandom = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
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

        public static readonly IFunction    FunctionSlice = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int     count;
            int     limit;
            int     index;
            Value   value = values[0];

            limit = value.Type == Value.DataType.ARRAY ? value.Fields.Count : value.AsString.Length;
            index = Math.Min ((int)values[1].AsNumber, limit);
            count = values.Count > 2 ? Math.Min ((int)values[2].AsNumber, limit - index) : limit - index;

            if (value.Type == Value.DataType.ARRAY)
                return value.Fields.GetRange (index, count);
            else
                return value.AsString.Substring (index, count);
        }, 2, 3);

        public static readonly IFunction    FunctionSort = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            IFunction                           callback = values.Count > 1 ? values[1].AsFunction : null;
            List<KeyValuePair<Value, Value>>    source = values[0].Fields;
            List<KeyValuePair<Value, Value>>    sorted = new List<KeyValuePair<Value, Value>> (source);

            if (callback != null)
                sorted.Sort ((a, b) => (int)callback.Execute (new Value[] {a.Value, b.Value}, scope, output).AsNumber);
            else
                sorted.Sort ((a, b) => a.Value.CompareTo (b.Value));

            return sorted;
        }, 1, 2);

        public static readonly IFunction    FunctionSplit = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return Array.ConvertAll (values[0].AsString.Split (new string[] {values[1].AsString}, StringSplitOptions.None), s => new StringValue (s));
        }, 2);

        public static readonly IFunction    FunctionSub = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber - values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionUnion = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Dictionary<Value, Value>    result = new Dictionary<Value, Value> ();

            foreach (Value value in values)
            {
                foreach (KeyValuePair<Value, Value> pair in value.Fields)
                    result[pair.Key] = pair.Value;
            }

            return result;
        }, 0, -1);

        public static readonly IFunction    FunctionUpperCase = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsString.ToUpperInvariant ();
        }, 1);

        public static readonly IFunction    FunctionXor = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int count = 0;

            foreach (Value value in values)
            {
                if (value.AsBoolean)
                    ++count;
            }

            return count == 1;
        });

        #endregion

        #region Attributes

        private static Dictionary<string, KeyValuePair<Document, DateTime>> includes = new Dictionary<string, KeyValuePair<Document, DateTime>> ();

        [ThreadStatic]
        private static Random                                               random = null;

        #endregion

        #region Methods

        public static void  Assign (Scope scope)
        {
            scope.Set ("abs", new FunctionValue (CommonFunctions.FunctionAbsolute), Scope.SetMode.ANYWHERE);
            scope.Set ("add", new FunctionValue (CommonFunctions.FunctionAdd), Scope.SetMode.ANYWHERE);
            scope.Set ("and", new FunctionValue (CommonFunctions.FunctionAnd), Scope.SetMode.ANYWHERE);
            scope.Set ("cat", new FunctionValue (CommonFunctions.FunctionCat), Scope.SetMode.ANYWHERE);
            scope.Set ("char", new FunctionValue (CommonFunctions.FunctionChar), Scope.SetMode.ANYWHERE);
            scope.Set ("cmp", new FunctionValue (CommonFunctions.FunctionCompare), Scope.SetMode.ANYWHERE);
            scope.Set ("cross", new FunctionValue (CommonFunctions.FunctionCross), Scope.SetMode.ANYWHERE);
            scope.Set ("div", new FunctionValue (CommonFunctions.FunctionDiv), Scope.SetMode.ANYWHERE);
            scope.Set ("eq", new FunctionValue (CommonFunctions.FunctionEqual), Scope.SetMode.ANYWHERE);
            scope.Set ("except", new FunctionValue (CommonFunctions.FunctionExcept), Scope.SetMode.ANYWHERE);
            scope.Set ("find", new FunctionValue (CommonFunctions.FunctionFind), Scope.SetMode.ANYWHERE);
            scope.Set ("flip", new FunctionValue (CommonFunctions.FunctionFlip), Scope.SetMode.ANYWHERE);
            scope.Set ("ge", new FunctionValue (CommonFunctions.FunctionGreaterEqual), Scope.SetMode.ANYWHERE);
            scope.Set ("gt", new FunctionValue (CommonFunctions.FunctionGreater), Scope.SetMode.ANYWHERE);
            scope.Set ("has", new FunctionValue (CommonFunctions.FunctionHas), Scope.SetMode.ANYWHERE);
            scope.Set ("include", new FunctionValue (CommonFunctions.FunctionInclude), Scope.SetMode.ANYWHERE);
            scope.Set ("join", new FunctionValue (CommonFunctions.FunctionJoin), Scope.SetMode.ANYWHERE);
            scope.Set ("lcase", new FunctionValue (CommonFunctions.FunctionLowerCase), Scope.SetMode.ANYWHERE);
            scope.Set ("le", new FunctionValue (CommonFunctions.FunctionLowerEqual), Scope.SetMode.ANYWHERE);
            scope.Set ("len", new FunctionValue (CommonFunctions.FunctionLength), Scope.SetMode.ANYWHERE);
            scope.Set ("lt", new FunctionValue (CommonFunctions.FunctionLower), Scope.SetMode.ANYWHERE);
            scope.Set ("map", new FunctionValue (CommonFunctions.FunctionMap), Scope.SetMode.ANYWHERE);
            scope.Set ("match", new FunctionValue (CommonFunctions.FunctionMatch), Scope.SetMode.ANYWHERE);
            scope.Set ("max", new FunctionValue (CommonFunctions.FunctionMaximum), Scope.SetMode.ANYWHERE);
            scope.Set ("min", new FunctionValue (CommonFunctions.FunctionMinimum), Scope.SetMode.ANYWHERE);
            scope.Set ("mod", new FunctionValue (CommonFunctions.FunctionMod), Scope.SetMode.ANYWHERE);
            scope.Set ("mul", new FunctionValue (CommonFunctions.FunctionMul), Scope.SetMode.ANYWHERE);
            scope.Set ("not", new FunctionValue (CommonFunctions.FunctionNot), Scope.SetMode.ANYWHERE);
            scope.Set ("or", new FunctionValue (CommonFunctions.FunctionOr), Scope.SetMode.ANYWHERE);
            scope.Set ("ord", new FunctionValue (CommonFunctions.FunctionOrd), Scope.SetMode.ANYWHERE);
            scope.Set ("rand", new FunctionValue (CommonFunctions.FunctionRandom), Scope.SetMode.ANYWHERE);
            scope.Set ("slice", new FunctionValue (CommonFunctions.FunctionSlice), Scope.SetMode.ANYWHERE);
            scope.Set ("sort", new FunctionValue (CommonFunctions.FunctionSort), Scope.SetMode.ANYWHERE);
            scope.Set ("split", new FunctionValue (CommonFunctions.FunctionSplit), Scope.SetMode.ANYWHERE);
            scope.Set ("sub", new FunctionValue (CommonFunctions.FunctionSub), Scope.SetMode.ANYWHERE);
            scope.Set ("ucase", new FunctionValue (CommonFunctions.FunctionUpperCase), Scope.SetMode.ANYWHERE);
            scope.Set ("union", new FunctionValue (CommonFunctions.FunctionUnion), Scope.SetMode.ANYWHERE);
            scope.Set ("xor", new FunctionValue (CommonFunctions.FunctionXor), Scope.SetMode.ANYWHERE);
        }

        #endregion
    }
}
