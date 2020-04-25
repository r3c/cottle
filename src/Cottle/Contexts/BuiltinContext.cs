using System.Collections.Generic;
using System.Linq;
using Cottle.Builtins;

namespace Cottle.Contexts
{
    /// <summary>
    /// Read-only context providing access to Cottle builtins.
    /// </summary>
    internal class BuiltinContext : IContext
    {
        public static readonly BuiltinContext Instance = new BuiltinContext();

        private static readonly IReadOnlyDictionary<Value, Value> Builtins =
            BuiltinFunctions.Instances.ToDictionary(instance => Value.FromString(instance.Key),
                instance => Value.FromFunction(instance.Value));

        private BuiltinContext()
        {
        }

        public Value this[Value symbol] =>
            BuiltinContext.Builtins.TryGetValue(symbol, out var value) ? value : Value.Undefined;
    }
}