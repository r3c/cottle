using System.Collections.Generic;
using System.Linq;
using Cottle.Builtins;
using Cottle.Values;

namespace Cottle.Contexts
{
    /// <summary>
    /// Read-only context providing access to Cottle builtins.
    /// </summary>
    internal class BuiltinContext : IContext
    {
        public static readonly BuiltinContext Instance = new BuiltinContext();

        private static readonly IReadOnlyDictionary<Value, Value> Builtins =
            BuiltinFunctions.Instances.ToDictionary(instance => (Value) instance.Key,
                instance => new FunctionValue(instance.Value) as Value);

        public Value this[Value symbol] => Builtins.TryGetValue(symbol, out var value) ? value : VoidValue.Instance;

        private BuiltinContext()
        {
        }
    }
}