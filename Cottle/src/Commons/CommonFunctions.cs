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

        public static readonly IFunction    FunctionAdd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return new NumberValue (values[0].AsNumber + values[1].AsNumber);
        }, 2);

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
                return (char)values[0].AsNumber;
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

        public static readonly IFunction    FunctionCount = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].Fields.Count;
        }, 1);

        public static readonly IFunction    FunctionDiv = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal denominator = values[1].AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return values[0].AsNumber / denominator;
        }, 2);

        public static readonly IFunction    FunctionEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            string  compare = values[0].AsString;
            int     i;

            for (i = 1; i < values.Count; ++i)
                if (string.Compare (values[i].AsString, compare) != 0)
                    return false;

            return true;
        }, 1, -1);

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

        public static readonly IFunction    FunctionLength = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsString.Length;
        }, 1);

        public static readonly IFunction    FunctionLower = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber < values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionLowerEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber <= values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionMap = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            KeyValuePair<Value, Value>[]    array = new KeyValuePair<Value, Value>[values[0].Fields.Count];
            IFunction                       callback = values[1].AsFunction;
            int                             i = 0;

            if (callback == null)
                return UndefinedValue.Instance;

            foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
                array[i++] = new KeyValuePair<Value, Value> (pair.Key, callback.Execute (new Value[] {pair.Value}, scope, output));

            return array;
        }, 2);

        public static readonly IFunction    FunctionMatch = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            try
            {
                return Regex.IsMatch (values[0].AsString, values[1].AsString);
            }
            catch
            {
                return UndefinedValue.Instance;
            }
        }, 2);

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

        public static readonly IFunction    FunctionOrd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            string  chr = values[0].AsString;

            if (chr.Length > 0)
                return (decimal)(int)chr[0];

            return 0;
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
            List<KeyValuePair<Value, Value>>    array = values[0].Fields;
            int                                 count;
            int                                 index;
            KeyValuePair<Value, Value>[]        slice;
            int                                 start;

            start = Math.Min ((int)values[1].AsNumber, array.Count);
            count = values.Count > 2 ? Math.Min ((int)values[2].AsNumber, array.Count - start) : array.Count - start;

            slice = new KeyValuePair<Value, Value>[count];

            for (index = 0; index < count; ++index)
                slice[index] = array[start + index];

            return slice;
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

        public static readonly IFunction    FunctionSub = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber - values[1].AsNumber;
        }, 2);

        #endregion

        #region Attributes

        [ThreadStatic]
        private static Random   random = null;

        #endregion

        #region Methods

        public static void  Assign (Document document)
        {
            document.Values["add"] = new FunctionValue (CommonFunctions.FunctionAdd);
            document.Values["cat"] = new FunctionValue (CommonFunctions.FunctionCat);
            document.Values["char"] = new FunctionValue (CommonFunctions.FunctionChar);
            document.Values["cmp"] = new FunctionValue (CommonFunctions.FunctionCompare);
            document.Values["count"] = new FunctionValue (CommonFunctions.FunctionCount);
            document.Values["div"] = new FunctionValue (CommonFunctions.FunctionDiv);
            document.Values["equal"] = new FunctionValue (CommonFunctions.FunctionEqual);
            document.Values["ge"] = new FunctionValue (CommonFunctions.FunctionGreaterEqual);
            document.Values["gt"] = new FunctionValue (CommonFunctions.FunctionGreater);
            document.Values["has"] = new FunctionValue (CommonFunctions.FunctionHas);
            document.Values["le"] = new FunctionValue (CommonFunctions.FunctionLowerEqual);
            document.Values["len"] = new FunctionValue (CommonFunctions.FunctionLength);
            document.Values["lt"] = new FunctionValue (CommonFunctions.FunctionLower);
            document.Values["map"] = new FunctionValue (CommonFunctions.FunctionMap);
            document.Values["match"] = new FunctionValue (CommonFunctions.FunctionMatch);
            document.Values["mod"] = new FunctionValue (CommonFunctions.FunctionMod);
            document.Values["mul"] = new FunctionValue (CommonFunctions.FunctionMul);
            document.Values["ord"] = new FunctionValue (CommonFunctions.FunctionOrd);
            document.Values["rand"] = new FunctionValue (CommonFunctions.FunctionRandom);
            document.Values["slice"] = new FunctionValue (CommonFunctions.FunctionSlice);
            document.Values["sort"] = new FunctionValue (CommonFunctions.FunctionSort);
            document.Values["sub"] = new FunctionValue (CommonFunctions.FunctionSub);
        }

        #endregion
    }
}
