using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Cottle.Functions;
using Cottle.Values;

namespace   Cottle.Commons
{
    public static class CommonFunctions
    {
        #region Constants

        public static readonly Function    FunctionAdd = new CallbackFunction (delegate (Argument[] arguments)
        {
            return new NumberValue (arguments[0].Value.AsNumber + arguments[1].Value.AsNumber);
        }, 2);

        public static readonly Function    FunctionCat = new CallbackFunction (delegate (Argument[] arguments)
        {
            StringBuilder   builder = new StringBuilder ();

            foreach (Argument argument in arguments)
                builder.Append (argument.Value.AsString);

            return builder.ToString ();
        });

        public static readonly Function    FunctionCount = new CallbackFunction (delegate (Argument[] arguments)
        {
            return arguments[0].Value.Fields.Count;
        }, 1);

        public static readonly Function    FunctionDiv = new CallbackFunction (delegate (Argument[] arguments)
        {
            decimal denominator = arguments[1].Value.AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return arguments[0].Value.AsNumber / denominator;
        }, 2);

        public static readonly Function    FunctionEqual = new CallbackFunction (delegate (Argument[] arguments)
        {
            string  compare = arguments[0].Value.AsString;
            int     i;

            for (i = 1; i < arguments.Length; ++i)
                if (string.Compare (arguments[i].Value.AsString, compare) != 0)
                    return BooleanValue.False;

            return BooleanValue.True;
        }, 1, -1);

        public static readonly Function    FunctionGreater = new CallbackFunction (delegate (Argument[] arguments)
        {
            return arguments[0].Value.AsNumber > arguments[1].Value.AsNumber;
        }, 2);

        public static readonly Function    FunctionGreaterEqual = new CallbackFunction (delegate (Argument[] arguments)
        {
            return arguments[0].Value.AsNumber >= arguments[1].Value.AsNumber;
        }, 2);

        public static readonly Function    FunctionHas = new CallbackFunction (delegate (Argument[] arguments)
        {
            Value  value = arguments[0].Value;
            int     i;

            for (i = 1; i < arguments.Length; ++i)
                if (!value.Has (arguments[i].Value))
                    return BooleanValue.False;

            return BooleanValue.True;
        }, 1, -1);

        public static readonly Function     FunctionJoin = new CallbackFunction (delegate (Argument[] arguments)
        {
            List<KeyValuePair<Value, Value>>    array = new List<KeyValuePair<Value, Value>> ();

            foreach (Argument argument in arguments)
                array.AddRange (argument.Value.Fields);

            return new ArrayValue (array);
        }, 1, -1);

        public static readonly Function    FunctionLower = new CallbackFunction (delegate (Argument[] arguments)
        {
            return arguments[0].Value.AsNumber < arguments[1].Value.AsNumber;
        }, 2);

        public static readonly Function    FunctionLowerEqual = new CallbackFunction (delegate (Argument[] arguments)
        {
            return arguments[0].Value.AsNumber <= arguments[1].Value.AsNumber;
        }, 2);

        public static readonly Function    FunctionMatch = new CallbackFunction (delegate (Argument[] arguments)
        {
            try
            {
                return new BooleanValue (Regex.IsMatch (arguments[0].Value.AsString, arguments[1].Value.AsString));
            }
            catch
            {
                return UndefinedValue.Instance;
            }
        }, 2);

        public static readonly Function    FunctionMod = new CallbackFunction (delegate (Argument[] arguments)
        {
            decimal denominator = arguments[1].Value.AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return arguments[0].Value.AsNumber % denominator;
        }, 2);

        public static readonly Function    FunctionMul = new CallbackFunction (delegate (Argument[] arguments)
        {
            return arguments[0].Value.AsNumber * arguments[1].Value.AsNumber;
        }, 2);

        public static readonly Function    FunctionRandom = new CallbackFunction (delegate (Argument[] arguments)
        {
            if (CommonFunctions.random == null)
                CommonFunctions.random = new Random ();

            switch (arguments.Length)
            {
                case 0:
                    return CommonFunctions.random.Next ();

                case 1:
                    return CommonFunctions.random.Next ((int)arguments[0].Value.AsNumber);

                default:
                    return CommonFunctions.random.Next ((int)arguments[0].Value.AsNumber, (int)arguments[1].Value.AsNumber);
            }
        }, 0, 2);

        public static readonly Function    FunctionSlice = new CallbackFunction (delegate (Argument[] arguments)
        {
            IList<KeyValuePair<Value, Value>> array = arguments[0].Value.Fields;
            int                                 count;
            int                                 index;
            IList<KeyValuePair<Value, Value>> slice;
            int                                 start;

            start = Math.Min ((int)arguments[1].Value.AsNumber, array.Count);
            count = arguments.Length > 2 ? Math.Min ((int)arguments[2].Value.AsNumber, array.Count - start) : array.Count - start;

            slice = new KeyValuePair<Value, Value>[count];

            for (index = 0; index < count; ++index)
                slice[index] = array[start + index];

            return new ArrayValue (slice);
        }, 2, 3);

        public static readonly Function    FunctionSub = new CallbackFunction (delegate (Argument[] arguments)
        {
            return arguments[0].Value.AsNumber - arguments[1].Value.AsNumber;
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
            document.Values["count"] = new FunctionValue (CommonFunctions.FunctionCount);
            document.Values["div"] = new FunctionValue (CommonFunctions.FunctionDiv);
            document.Values["equal"] = new FunctionValue (CommonFunctions.FunctionEqual);
            document.Values["ge"] = new FunctionValue (CommonFunctions.FunctionGreaterEqual);
            document.Values["gt"] = new FunctionValue (CommonFunctions.FunctionGreater);
            document.Values["has"] = new FunctionValue (CommonFunctions.FunctionHas);
            document.Values["join"] = new FunctionValue (CommonFunctions.FunctionJoin);
            document.Values["le"] = new FunctionValue (CommonFunctions.FunctionLowerEqual);
            document.Values["lt"] = new FunctionValue (CommonFunctions.FunctionLower);
            document.Values["match"] = new FunctionValue (CommonFunctions.FunctionMatch);
            document.Values["mod"] = new FunctionValue (CommonFunctions.FunctionMod);
            document.Values["mul"] = new FunctionValue (CommonFunctions.FunctionMul);
            document.Values["rand"] = new FunctionValue (CommonFunctions.FunctionRandom);
            document.Values["slice"] = new FunctionValue (CommonFunctions.FunctionSlice);
            document.Values["sub"] = new FunctionValue (CommonFunctions.FunctionSub);
        }

        #endregion
    }
}
