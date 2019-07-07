using System;
using System.Collections.Generic;
using Cottle.Contexts;
using Cottle.Contexts.Monitor;

namespace Cottle
{
    /// <summary>
    /// Context construction helper.
    /// </summary>
    public static class Context
    {
        /// <summary>
        /// Get empty context instance.
        /// </summary>
        public static IContext Empty => EmptyContext.Instance;

        /// <summary>
        /// Create a new context with Cottle builtins and given symbols. Any symbol sharing the same name than a Cottle
        /// builtin will get precedence and hide it.
        /// </summary>
        /// <param name="symbols">Symbols dictionary</param>
        /// <returns>Cottle context</returns>
        public static IContext CreateBuiltin(IReadOnlyDictionary<Value, Value> symbols)
        {
            return new CascadeContext(Context.CreateCustom(symbols), BuiltinContext.Instance);
        }

        /// <summary>
        /// Create a new context with given symbols.
        /// </summary>
        /// <param name="symbols">Symbols dictionary</param>
        /// <returns>Cottle context</returns>
        public static IContext CreateCustom(IReadOnlyDictionary<Value, Value> symbols)
        {
            return new DictionaryContext(symbols);
        }

        /// <summary>
        /// Create a new context with monitoring capabilities.
        /// </summary>
        /// <param name="context">Context to be monitored</param>
        /// <returns>Cottle context</returns>
        public static (IContext, ISymbolUsage) CreateMonitor(IContext context)
        {
            var monitor = new MonitorContext(context);

            return (monitor, monitor.Usage);
        }

        /// <summary>
        /// Create a new empty context.
        /// </summary>
        /// <returns>Cottle context</returns>
        [Obsolete("Use `Context.Empty`.")]
        public static IContext CreateEmpty()
        {
            return EmptyContext.Instance;
        }
    }
}