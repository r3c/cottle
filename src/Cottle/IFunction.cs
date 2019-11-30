using System;
using System.Collections.Generic;
using System.IO;

namespace Cottle
{
    /// <summary>
    /// Executable Cottle function.
    /// </summary>
    public interface IFunction : IComparable<IFunction>, IEquatable<IFunction>
    {
        /// <summary>
        /// True if function is pure (will always produce the same result for the same arguments and doesn't rely on or
        /// trigger any side effect), or false otherwise.
        /// </summary>
        bool IsPure { get; }

        /// <summary>
        /// Invoke function with given argument on current document output.
        /// </summary>
        /// <param name="state">State to be passed to nested function calls</param>
        /// <param name="arguments">Function argument values</param>
        /// <param name="output">Document output text writer</param>
        /// <returns>Function execution result</returns>
        Value Invoke(object state, IReadOnlyList<Value> arguments, TextWriter output);
    }
}