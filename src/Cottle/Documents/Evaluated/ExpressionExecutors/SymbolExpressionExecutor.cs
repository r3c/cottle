using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.ExpressionExecutors
{
    internal class SymbolExpressionExecutor : IExpressionExecutor
    {
        private readonly Func<Frame, Value> _getter;

        public SymbolExpressionExecutor(Symbol symbol, StoreMode mode)
        {
            var index = symbol.Index;

            _getter = mode switch
            {
                StoreMode.Global => frame => frame.Globals[index],
                StoreMode.Local => frame => frame.Locals[index],
                _ => throw new ArgumentOutOfRangeException(nameof(symbol))
            };
        }

        public Value Execute(Frame frame, TextWriter output)
        {
            return _getter(frame);
        }
    }
}