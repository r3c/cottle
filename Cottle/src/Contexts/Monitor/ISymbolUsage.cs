using System.Collections.Generic;

namespace Cottle.Contexts.Monitor
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

        /// <summary>
        /// Group field usage statistics by field name. Result is a single statistics object per field name, where all
        /// child field accesses are combined and only first field value is retained.
        /// </summary>
        /// <returns>Usage statistics by field name</returns>
        IReadOnlyDictionary<Value, ISymbolUsage> GroupFieldUsages();
    }
}