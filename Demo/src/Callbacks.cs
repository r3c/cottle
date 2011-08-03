using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Cottle;
using Cottle.Values;

namespace   Demo
{
    static class    Callbacks
    {
        public static IValue    Add (Argument[] arguments)
        {
            return new NumberValue (arguments[0].Value.AsNumber + arguments[1].Value.AsNumber);
        }

        public static IValue    Contains (Argument[] arguments)
        {
            IValue  value = arguments[0].Value;
            int     i;

            for (i = 1; i < arguments.Length; ++i)
                if (!value.Has (arguments[i].Value.AsString))
                    return BooleanValue.False;

            return BooleanValue.True;
        }

        public static IValue    Count (Argument[] arguments)
        {
            return new NumberValue (arguments[0].Value.Children.Count);
        }

        public static IValue    Div (Argument[] arguments)
        {
            decimal denominator = arguments[1].Value.AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return new NumberValue (arguments[0].Value.AsNumber / denominator);
        }

        public static IValue    Equal (Argument[] arguments)
        {
            string  compare = arguments[0].Value.AsString;
            int     i;

            for (i = 1; i < arguments.Length; ++i)
                if (string.Compare (arguments[i].Value.AsString, compare) != 0)
                    return BooleanValue.False;

            return BooleanValue.True;
        }

        public static IValue    Greater (Argument[] arguments)
        {
            return new BooleanValue (arguments[0].Value.AsNumber > arguments[1].Value.AsNumber);
        }

        public static IValue    GreaterEqual (Argument[] arguments)
        {
            return new BooleanValue (arguments[0].Value.AsNumber >= arguments[1].Value.AsNumber);
        }

        public static IValue    Lower (Argument[] arguments)
        {
            return new BooleanValue (arguments[0].Value.AsNumber < arguments[1].Value.AsNumber);
        }

        public static IValue    LowerEqual (Argument[] arguments)
        {
            return new BooleanValue (arguments[0].Value.AsNumber <= arguments[1].Value.AsNumber);
        }

        public static IValue    Match (Argument[] arguments)
        {
            try
            {
                return new BooleanValue (Regex.IsMatch (arguments[0].Value.AsString, arguments[1].Value.AsString));
            }
            catch
            {
                return UndefinedValue.Instance;
            }
        }

        public static IValue    Mod (Argument[] arguments)
        {
            decimal denominator = arguments[1].Value.AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return new NumberValue (arguments[0].Value.AsNumber % denominator);
        }

        public static IValue    Mul (Argument[] arguments)
        {
            return new NumberValue (arguments[0].Value.AsNumber * arguments[1].Value.AsNumber);
        }

        public static IValue    Slice (Argument[] arguments)
        {
            IList<KeyValuePair<IValue, IValue>> array = arguments[0].Value.Children;
            int                                 count;
            int                                 index;
            IList<KeyValuePair<IValue, IValue>> slice;
            int                                 start;

            start = Math.Min ((int)arguments[1].Value.AsNumber, array.Count);
            count = arguments.Length > 2 ? Math.Min ((int)arguments[2].Value.AsNumber, array.Count - start) : array.Count - start;

            slice = new KeyValuePair<IValue, IValue>[count];

            for (index = 0; index < count; ++index)
                slice[index] = array[start + index];

            return new ArrayValue (slice);
        }

        public static IValue    Sub (Argument[] arguments)
        {
            return new NumberValue (arguments[0].Value.AsNumber - arguments[1].Value.AsNumber);
        }
    }
}
