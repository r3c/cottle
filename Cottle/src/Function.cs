using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Functions;

namespace Cottle
{
    public static class Function
    {
        /// <summary>
        /// Create a function taking a number of input arguments enclosed within a given range.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <param name="min">Minimum number of accepted arguments</param>
        /// <param name="max">Maximum number of accepted arguments</param>
        /// <returns>Function instance</returns>
        public static IFunction Create(Func<object, IReadOnlyList<Value>, TextWriter, Value> callback, int min, int max)
        {
            return new CallbackFunction(false, callback, min, max);
        }

        /// <summary>
        /// Create a function taking an exact number of input arguments.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <param name="count">Number of accepted arguments</param>
        /// <returns>Function instance</returns>
        public static IFunction Create(Func<object, IReadOnlyList<Value>, TextWriter, Value> callback, int count)
        {
            return Function.Create(callback, count, count);
        }

        /// <summary>
        /// Create a function accepting any number of input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction Create(Func<object, IReadOnlyList<Value>, TextWriter, Value> callback)
        {
            return Function.Create(callback, 0, int.MaxValue);
        }

        /// <summary>
        /// Create an impure function (a function causing or relying on side effects) taking one input argument.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction Create1(Func<object, Value, TextWriter, Value> callback)
        {
            return new CallbackFunction(false, (state, arguments, output) => callback(state, arguments[0], output), 1,
                1);
        }

        /// <summary>
        /// Create an impure first order function (a function causing or relying on side effects) taking two input
        /// arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction Create2(Func<object, Value, Value, TextWriter, Value> callback)
        {
            return new CallbackFunction(false,
                (state, arguments, output) => callback(state, arguments[0], arguments[1], output), 2, 2);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking a number of input arguments
        /// enclosed within a given range.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <param name="min">Minimum number of accepted arguments</param>
        /// <param name="max">Maximum number of accepted arguments</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePure(Func<object, IReadOnlyList<Value>, Value> callback, int min, int max)
        {
            return new CallbackFunction(true, (state, arguments, _) => callback(state, arguments), min, max);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking an exact number of input arguments.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <param name="count">Number of accepted arguments</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePure(Func<object, IReadOnlyList<Value>, Value> callback, int count)
        {
            return Function.CreatePure(callback, count, count);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) accepting any number of input arguments.
        /// </summary>
        /// <param name="callback">Pure execution callback, must not depend on any side effect</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePure(Func<object, IReadOnlyList<Value>, Value> callback)
        {
            return Function.CreatePure(callback, 0, int.MaxValue);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking one input argument.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePure1(Func<object, Value, Value> callback)
        {
            return new CallbackFunction(true, (state, arguments, _) => callback(state, arguments[0]), 1, 1);
        }

        /// <summary>
        /// Create a pure function (not relying nor causing any side effect) taking two input arguments.
        /// </summary>
        /// <param name="callback">Execution callback</param>
        /// <returns>Function instance</returns>
        public static IFunction CreatePure2(Func<object, Value, Value, Value> callback)
        {
            return new CallbackFunction(true, (state, arguments, _) => callback(state, arguments[0], arguments[1]), 2,
                2);
        }
    }
}