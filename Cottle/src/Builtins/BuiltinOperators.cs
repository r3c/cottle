using System;
using Cottle.Functions;
using Cottle.Values;

namespace Cottle.Builtins
{
	static class BuiltinOperators
	{
		public static readonly IFunction operatorAdd = new NativeFunction ((v) => v[0].AsNumber + v[1].AsNumber, 2);

		public static readonly IFunction operatorAnd = new NativeFunction ((values) =>
		{
			foreach (Value value in values)
			{
				if (!value.AsBoolean)
					return false;
			}

			return true;
		});

		public static readonly IFunction operatorDiv = new NativeFunction ((values) =>
		{
			decimal denominator;

			denominator = values[1].AsNumber;

			if (denominator == 0)
				return VoidValue.Instance;

			return values[0].AsNumber / denominator;
		}, 2);

		public static readonly IFunction operatorEqual = new NativeFunction ((values) =>
		{
			Value first;

			first = values[0];

			for (int i = 1; i < values.Count; ++i)
				if (values[i].CompareTo (first) != 0)
					return false;

			return true;
		}, 1, -1);

		public static readonly IFunction operatorGreaterEqual = new NativeFunction ((v) => v[0].CompareTo (v[1]) >= 0, 2);

		public static readonly IFunction operatorGreaterThan = new NativeFunction ((v) => v[0].CompareTo (v[1]) > 0, 2);

		public static readonly IFunction operatorLowerEqual = new NativeFunction ((v) => v[0].CompareTo (v[1]) <= 0, 2);

		public static readonly IFunction operatorLowerThan = new NativeFunction ((v) => v[0].CompareTo (v[1]) < 0, 2);

		public static readonly IFunction operatorMod = new NativeFunction ((values) =>
		{
			decimal denominator;

			denominator = values[1].AsNumber;

			if (denominator == 0)
				return VoidValue.Instance;

			return values[0].AsNumber % denominator;
		}, 2);

		public static readonly IFunction operatorMul = new NativeFunction ((v) => v[0].AsNumber * v[1].AsNumber, 2);

		public static readonly IFunction operatorNot = new NativeFunction ((v) => !v[0].AsBoolean, 1);

		public static readonly IFunction operatorNotEqual = new NativeFunction ((values) =>
		{
			Value first;

			first = values[0];

			for (int i = 1; i < values.Count; ++i)
				if (values[i].CompareTo (first) == 0)
					return false;

			return true;
		}, 1, -1);

		public static readonly IFunction operatorOr = new NativeFunction ((values) =>
		{
			foreach (Value value in values)
			{
				if (value.AsBoolean)
					return true;
			}

			return false;
		});

		public static readonly IFunction operatorSub = new NativeFunction ((v) => v[0].AsNumber - v[1].AsNumber, 2);
	}
}
