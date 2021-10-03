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
        /// Create a new context with custom variables and Cottle builtins used as a fallback. Any variable sharing the
        /// same name than a Cottle builtin will get precedence and hide it.
        /// </summary>
        /// <param name="custom">Custom symbols</param>
        /// <returns>Cottle context</returns>
        public static IContext CreateBuiltin(IContext custom)
        {
            return new CascadeContext(custom, BuiltinContext.Instance);
        }

        /// <summary>
        /// Create a new context with given variables dictionary and Cottle builtins used as a fallback. Any variable
        /// sharing the same name than a Cottle builtin will get precedence and hide it.
        /// </summary>
        /// <param name="symbols">Symbols dictionary</param>
        /// <returns>Cottle context</returns>
        public static IContext CreateBuiltin(IReadOnlyDictionary<Value, Value> symbols)
        {
            return new CascadeContext(Context.CreateCustom(symbols), BuiltinContext.Instance);
        }

        /// <summary>
        /// Create a new context using two underlying instances for retrieving variables. Variable is first searched by
        /// key within primary instance, then fallback instance if value was void in primary one.
        /// </summary>
        /// <param name="primary">Primary context</param>
        /// <param name="fallback">Fallback context</param>
        /// <returns>Cottle context</returns>
        public static IContext CreateCascade(IContext primary, IContext fallback)
        {
            return new CascadeContext(primary, fallback);
        }

        /// <summary>
        /// Create a new context using given callback function as a variable getter.
        /// </summary>
        /// <param name="callback">Variable getter function</param>
        /// <returns>Cottle context</returns>
        public static IContext CreateCustom(Func<Value, Value> callback)
        {
            return new CallbackContext(callback);
        }

        /// <summary>
        /// Create a new context filled with given dictionary of variables.
        /// </summary>
        /// <param name="symbols">Symbols dictionary</param>
        /// <returns>Cottle context</returns>
        public static IContext CreateCustom(IReadOnlyDictionary<Value, Value> symbols)
        {
            return new DictionaryContext(symbols);
        }

        /// <summary>
        /// Create a new spying context to keep track of accessed variables and fields.
        /// </summary>
        /// <param name="source">Source context to spy on</param>
        /// <returns>Cottle context</returns>
        public static ISpyContext CreateSpy(IContext source)
        {
            return new SpyContext(source);
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