using System.Collections.Generic;

namespace Cottle.Stores.Monitor
{
    /// <summary>
    /// Usage statistics of a Cottle symbol.
    /// </summary>
    public interface ISymbolUsage
    {
        /// <summary>
        /// Usage statistics for accesses to this symbol fields.
        /// </summary>
        IReadOnlyDictionary<Value, IReadOnlyList<ISymbolUsage>> Fields { get; }

        /// <summary>
        /// Value of the field when it was accessed.
        /// </summary>
        Value Value { get; }
    }
}
