using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Functions;

namespace Cottle
{
    public static class Function
    {
        /// <summary>
        /// Function always returning an empty result.
        /// </summary>
        internal static readonly IFunction Empty = Function.CreatePure0(_ => Value.Undefined);

        /// <summary>
        /// Create a function taking a number of input arguments enclosed within a given range.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <param name="min">Minimum number of accepted arguments</param>
        /// <param name="max">Maximum number of accepted arguments</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreateNativeMinMax`")]
        public static IFunction Create(Func<object, IReadOnlyList<Value>, TextWriter, Value> callback, int min, int max)
        {
            return Function.CreateNativeMinMax(callback, min, max);
        }

        /// <summary>
        /// Create a function taking an exact number of input arguments.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <param name="count">Number of accepted arguments</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreateNativeExact`")]
        public static IFunction Create(Func<object, IReadOnlyList<Value>, TextWriter, Value> callback, int count)
        {
            return Function.CreateNativeExact(callback, count);
        }

        /// <summary>
        /// Create a function accepting any number of input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreateNativeVariadic`")]
        public static IFunction Create(Func<object, IReadOnlyList<Value>, TextWriter, Value> callback)
        {
            return Function.CreateNativeVariadic(callback);
        }

        /// <summary>
        /// Create an impure function (a function causing or relying on side effects) taking zero input argument.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreateNative0`")]
        public static IFunction Create0(Func<object, TextWriter, Value> callback)
        {
            return Function.CreateNative0(callback);
        }

        /// <summary>
        /// Create an impure function (a function causing or relying on side effects) taking one input argument.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreateNative1`")]
        public static IFunction Create1(Func<object, Value, TextWriter, Value> callback)
        {
            return Function.CreateNative1(callback);
        }

        /// <summary>
        /// Create an impure function (a function causing or relying on side effects) taking two input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreateNative2`")]
        public static IFunction Create2(Func<object, Value, Value, TextWriter, Value> callback)
        {
            return Function.CreateNative2(callback);
        }

        /// <summary>
        /// Create an impure function (a function causing or relying on side effects) taking three input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreateNative3`")]
        public static IFunction Create3(Func<object, Value, Value, Value, TextWriter, Value> callback)
        {
            return Function.CreateNative3(callback);
        }

        /// <summary>
        /// Create an impure function (a function causing or relying on side effects) taking zero input argument.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreateNative0(Func<IRuntime, TextWriter, Value> callback)
        {
            return new FiniteFunction(false, callback, null, null, null);
        }

        /// <summary>
        /// Create an impure function (a function causing or relying on side effects) taking one input argument.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreateNative1(Func<IRuntime, Value, TextWriter, Value> callback)
        {
            return new FiniteFunction(false, null, callback, null, null);
        }

        /// <summary>
        /// Create an impure function (a function causing or relying on side effects) taking two input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreateNative2(Func<IRuntime, Value, Value, TextWriter, Value> callback)
        {
            return new FiniteFunction(false, null, null, callback, null);
        }

        /// <summary>
        /// Create an impure function (a function causing or relying on side effects) taking three input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreateNative3(Func<IRuntime, Value, Value, Value, TextWriter, Value> callback)
        {
            return new FiniteFunction(false, null, null, null, callback);
        }

        /// <summary>
        /// Create a function taking an exact number of input arguments.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <param name="count">Number of accepted arguments</param>
        /// <returns>Function instance</returns>
        public static IFunction CreateNativeExact(Func<IRuntime, IReadOnlyList<Value>, TextWriter, Value> callback,
            int count)
        {
            return Function.CreateNativeMinMax(callback, count, count);
        }

        /// <summary>
        /// Create a function taking a number of input arguments enclosed within a given range.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <param name="min">Minimum number of accepted arguments</param>
        /// <param name="max">Maximum number of accepted arguments</param>
        /// <returns>Function instance</returns>
        public static IFunction CreateNativeMinMax(Func<IRuntime, IReadOnlyList<Value>, TextWriter, Value> callback,
            int min, int max)
        {
            return new ArbitraryFunction(false, callback, min, max);
        }

        /// <summary>
        /// Create a function accepting any number of input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreateNativeVariadic(Func<IRuntime, IReadOnlyList<Value>, TextWriter, Value> callback)
        {
            return Function.CreateNativeMinMax(callback, 0, int.MaxValue);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking a number of input arguments
        /// enclosed within a given range.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <param name="min">Minimum number of accepted arguments</param>
        /// <param name="max">Maximum number of accepted arguments</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreatePureMinMax`")]
        public static IFunction CreatePure(Func<object?, IReadOnlyList<Value>, Value> callback, int min, int max)
        {
            return Function.CreatePureMinMax(callback, min, max);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking an exact number of input arguments.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <param name="count">Number of accepted arguments</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreatePureExact`")]
        public static IFunction CreatePure(Func<object?, IReadOnlyList<Value>, Value> callback, int count)
        {
            return Function.CreatePureExact(callback, count);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) accepting any number of input arguments.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <returns>Function instance</returns>
        [Obsolete("Replace by `Function.CreatePureVariadic`")]
        public static IFunction CreatePure(Func<object?, IReadOnlyList<Value>, Value> callback)
        {
            return Function.CreatePureVariadic(callback);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking zero input argument.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePure0(Func<object?, Value> callback)
        {
            return new FiniteFunction(true, (state, _) => callback(state), null, null, null);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking one input argument.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePure1(Func<object?, Value, Value> callback)
        {
            return new FiniteFunction(true, null, (state, argument0, _) => callback(state, argument0), null, null);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking two input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePure2(Func<object?, Value, Value, Value> callback)
        {
            return new FiniteFunction(true, null, null,
                (state, argument0, argument1, _) => callback(state, argument0, argument1), null);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking three input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePure3(Func<object?, Value, Value, Value, Value> callback)
        {
            return new FiniteFunction(true, null, null, null,
                (state, argument0, argument1, argument2, _) => callback(state, argument0, argument1, argument2));
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking an exact number of input arguments.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <param name="count">Number of accepted arguments</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePureExact(Func<object?, IReadOnlyList<Value>, Value> callback, int count)
        {
            return Function.CreatePureMinMax(callback, count, count);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking a number of input arguments
        /// enclosed within a given range.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <param name="min">Minimum number of accepted arguments</param>
        /// <param name="max">Maximum number of accepted arguments</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePureMinMax(Func<object?, IReadOnlyList<Value>, Value> callback, int min, int max)
        {
            return new ArbitraryFunction(true, (state, arguments, _) => callback(state, arguments), min, max);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) accepting any number of input arguments.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePureVariadic(Func<object?, IReadOnlyList<Value>, Value> callback)
        {
            return Function.CreatePureMinMax(callback, 0, int.MaxValue);
        }
    }
}