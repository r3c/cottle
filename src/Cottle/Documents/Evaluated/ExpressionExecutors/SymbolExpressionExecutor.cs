using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.ExpressionExecutors
{
    internal class SymbolExpressionExecutor : IExpressionExecutor
    {
        private readonly Func<Runtime, Frame, Value> _getter;

        public SymbolExpressionExecutor(Symbol symbol, StoreMode mode)
        {
            var index = symbol.Index;

            _getter = mode switch
            {
                StoreMode.Global => (state, _) => state.GlobalValues[index],
                StoreMode.Local => (_, frame) => frame.Locals[index],
                _ => throw new ArgumentOutOfRangeException(nameof(symbol))
            };
        }

        public Value Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            return _getter(runtime, frame);
        }
    }
}