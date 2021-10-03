using System.Collections.Generic;

namespace Cottle
{
    public interface ISpyRecord
    {
        /// <summary>
        /// Latest observed value of spied variable.
        /// </summary>
        Value Value { get; }

        /// <summary>
        /// Spy field by key from spied variable.
        /// </summary>
        /// <param name="key">Field key</param>
        /// <returns>Spy record for given field</returns>
        ISpyRecord SpyField(Value key);

        /// <summary>
        /// Spy all fields from spied variable.
        /// </summary>
        /// <returns>Dictionary of field keys and associated spy records</returns>
        IReadOnlyDictionary<Value, ISpyRecord> SpyFields();
    }
}