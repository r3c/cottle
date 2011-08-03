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
            if (arguments.Length != 2)
                return UndefinedValue.Instance;

            return new NumberValue (arguments[0] ().AsNumber + arguments[1] ().AsNumber);
        }

        public static IValue    Contains (Argument[] arguments)
        {
            IValue  value;
            int     i;

            if (arguments.Length < 1)
                return UndefinedValue.Instance;

            value = arguments[0] ();

            for (i = 1; i < arguments.Length; ++i)
                if (!value.Has (arguments[i] ().AsString))
                    return BooleanValue.False;

            return BooleanValue.True;
        }

        public static IValue    Count (Argument[] arguments)
        {
            if (arguments.Length != 1)
                return UndefinedValue.Instance;

            return new NumberValue (arguments[0] ().Children.Count);
        }

        public static IValue    Div (Argument[] arguments)
        {
            decimal denominator;

            if (arguments.Length != 2)
                return UndefinedValue.Instance;

            denominator = arguments[1] ().AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return new NumberValue (arguments[0] ().AsNumber / denominator);
        }

        public static IValue    Equal (Argument[] arguments)
        {
            string  compare;
            int     i;

            if (arguments.Length < 1)
                return BooleanValue.False;

            compare = arguments[0] ().AsString;

            for (i = 1; i < arguments.Length; ++i)
                if (string.Compare (arguments[i] ().AsString, compare) != 0)
                    return BooleanValue.False;

            return BooleanValue.True;
        }

        public static IValue    Greater (Argument[] arguments)
        {
            if (arguments.Length != 2)
                return BooleanValue.False;

            return new BooleanValue (arguments[0] ().AsNumber > arguments[1] ().AsNumber);
        }

        public static IValue    GreaterEqual (Argument[] arguments)
        {
            if (arguments.Length != 2)
                return BooleanValue.False;

            return new BooleanValue (arguments[0] ().AsNumber >= arguments[1] ().AsNumber);
        }

        public static IValue    Lower (Argument[] arguments)
        {
            if (arguments.Length != 2)
                return UndefinedValue.Instance;

            return new BooleanValue (arguments[0] ().AsNumber < arguments[1] ().AsNumber);
        }

        public static IValue    LowerEqual (Argument[] arguments)
        {
            if (arguments.Length != 2)
                return UndefinedValue.Instance;

            return new BooleanValue (arguments[0] ().AsNumber <= arguments[1] ().AsNumber);
        }

        public static IValue    Match (Argument[] arguments)
        {
            if (arguments.Length != 2)
                return UndefinedValue.Instance;

            try
            {
                return new BooleanValue (Regex.IsMatch (arguments[0] ().AsString, arguments[1] ().AsString));
            }
            catch
            {
                return UndefinedValue.Instance;
            }
        }

        public static IValue    Mod (Argument[] arguments)
        {
            decimal denominator;

            if (arguments.Length != 2)
                return UndefinedValue.Instance;

            denominator = arguments[1] ().AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return new NumberValue (arguments[0] ().AsNumber % denominator);
        }

        public static IValue    Mul (Argument[] arguments)
        {
            if (arguments.Length != 2)
                return UndefinedValue.Instance;

            return new NumberValue (arguments[0] ().AsNumber * arguments[1] ().AsNumber);
        }

        public static IValue    Slice (Argument[] arguments)
        {
            IList<KeyValuePair<IValue, IValue>> array;
            int                                 count;
            int                                 index;
            IList<KeyValuePair<IValue, IValue>> slice;
            int                                 start;

            if (arguments.Length < 2 || arguments.Length > 3)
                return UndefinedValue.Instance;

            array = arguments[0] ().Children;

            start = Math.Min ((int)arguments[1] ().AsNumber, array.Count);
            count = arguments.Length > 2 ? Math.Min ((int)arguments[2] ().AsNumber, array.Count - start) : array.Count - start;

            slice = new KeyValuePair<IValue, IValue>[count];

            for (index = 0; index < count; ++index)
                slice[index] = array[start + index];

            return new ArrayValue (slice);
        }

        public static IValue    Sub (Argument[] arguments)
        {
            if (arguments.Length != 2)
                return UndefinedValue.Instance;

            return new NumberValue (arguments[0] ().AsNumber - arguments[1] ().AsNumber);
        }
	}
}
