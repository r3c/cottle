using System.Collections.Generic;

namespace Cottle
{
    /// <summary>
    /// Context with spying ability, able to observed accessed values and fields during rendering.
    /// </summary>
    public interface ISpyContext : IContext
    {
        /// <summary>
        /// Spy variable by key.
        /// </summary>
        /// <param name="key">Variable key</param>
        /// <returns>Spy record for given variable</returns>
        ISpyRecord SpyVariable(Value key);

        /// <summary>
        /// Spy all accessed variables.
        /// </summary>
        /// <returns>Dictionary of variable keys and associated spy records</returns>
        IReadOnlyDictionary<Value, ISpyRecord> SpyVariables();
    }
}