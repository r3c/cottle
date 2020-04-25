using System;

namespace Cottle.Builtins
{
    internal static class BuiltinOperators
    {
        public static readonly IFunction OperatorAdd =
            Function.CreatePure2((state, lhs, rhs) => lhs.AsNumber + rhs.AsNumber);

        public static readonly IFunction OperatorAnd = Function.CreatePure((state, arguments) =>
        {
            foreach (var argument in arguments)
            {
                if (!argument.AsBoolean)
                    return false;
            }

            return true;
        });

        public static readonly IFunction OperatorDiv = Function.CreatePure2((state, lhs, rhs) =>
        {
            var denominator = rhs.AsNumber;

            if (Math.Abs(denominator) < double.Epsilon)
                return Value.Undefined;

            return lhs.AsNumber / denominator;
        });

        public static readonly IFunction OperatorEqual = Function.CreatePure((state, arguments) =>
        {
            var first = arguments[0];

            for (var i = 1; i < arguments.Count; ++i)
            {
                if (arguments[i].CompareTo(first) != 0)
                    return false;
            }

            return true;
        }, 1, int.MaxValue);

        public static readonly IFunction OperatorGreaterEqual =
            Function.CreatePure2((state, lhs, rhs) => lhs.CompareTo(rhs) >= 0);

        public static readonly IFunction OperatorGreaterThan =
            Function.CreatePure2((state, lhs, rhs) => lhs.CompareTo(rhs) > 0);

        public static readonly IFunction OperatorLowerEqual =
            Function.CreatePure2((state, lhs, rhs) => lhs.CompareTo(rhs) <= 0);

        public static readonly IFunction OperatorLowerThan =
            Function.CreatePure2((state, lhs, rhs) => lhs.CompareTo(rhs) < 0);

        public static readonly IFunction OperatorMod = Function.CreatePure2((state, lhs, rhs) =>
        {
            var denominator = rhs.AsNumber;

            if (Math.Abs(denominator) < double.Epsilon)
                return Value.Undefined;

            return lhs.AsNumber % denominator;
        });

        public static readonly IFunction OperatorMul =
            Function.CreatePure2((state, lhs, rhs) => lhs.AsNumber * rhs.AsNumber);

        public static readonly IFunction OperatorNot = Function.CreatePure1((state, value) => !value.AsBoolean);

        public static readonly IFunction OperatorNotEqual = Function.CreatePure((state, arguments) =>
        {
            var first = arguments[0];

            for (var i = 1; i < arguments.Count; ++i)
            {
                if (arguments[i].CompareTo(first) == 0)
                    return false;
            }

            return true;
        }, 1, int.MaxValue);

        public static readonly IFunction OperatorOr = Function.CreatePure((state, arguments) =>
        {
            foreach (var argument in arguments)
            {
                if (argument.AsBoolean)
                    return true;
            }

            return false;
        });

        public static readonly IFunction OperatorSub =
            Function.CreatePure2((state, lhs, rhs) => lhs.AsNumber - rhs.AsNumber);
    }
}