using Cottle.Functions;
using Cottle.Values;

namespace Cottle.Builtins
{
    internal static class BuiltinOperators
    {
        public static readonly IFunction OperatorAdd = new NativeFunction(v => v[0].AsNumber + v[1].AsNumber, 2);

        public static readonly IFunction OperatorAnd = new NativeFunction(values =>
        {
            foreach (var value in values)
                if (!value.AsBoolean)
                    return false;

            return true;
        });

        public static readonly IFunction OperatorDiv = new NativeFunction(values =>
        {
            var denominator = values[1].AsNumber;

            if (denominator == 0)
                return VoidValue.Instance;

            return values[0].AsNumber / denominator;
        }, 2);

        public static readonly IFunction OperatorEqual = new NativeFunction(values =>
        {
            var first = values[0];

            for (var i = 1; i < values.Count; ++i)
                if (values[i].CompareTo(first) != 0)
                    return false;

            return true;
        }, 1, -1);

        public static readonly IFunction OperatorGreaterEqual = new NativeFunction(v => v[0].CompareTo(v[1]) >= 0, 2);

        public static readonly IFunction OperatorGreaterThan = new NativeFunction(v => v[0].CompareTo(v[1]) > 0, 2);

        public static readonly IFunction OperatorLowerEqual = new NativeFunction(v => v[0].CompareTo(v[1]) <= 0, 2);

        public static readonly IFunction OperatorLowerThan = new NativeFunction(v => v[0].CompareTo(v[1]) < 0, 2);

        public static readonly IFunction OperatorMod = new NativeFunction(values =>
        {
            var denominator = values[1].AsNumber;

            if (denominator == 0)
                return VoidValue.Instance;

            return values[0].AsNumber % denominator;
        }, 2);

        public static readonly IFunction OperatorMul = new NativeFunction(v => v[0].AsNumber * v[1].AsNumber, 2);

        public static readonly IFunction OperatorNot = new NativeFunction(v => !v[0].AsBoolean, 1);

        public static readonly IFunction OperatorNotEqual = new NativeFunction(values =>
        {
            var first = values[0];

            for (var i = 1; i < values.Count; ++i)
                if (values[i].CompareTo(first) == 0)
                    return false;

            return true;
        }, 1, -1);

        public static readonly IFunction OperatorOr = new NativeFunction(values =>
        {
            foreach (var value in values)
                if (value.AsBoolean)
                    return true;

            return false;
        });

        public static readonly IFunction OperatorSub = new NativeFunction(v => v[0].AsNumber - v[1].AsNumber, 2);
    }
}