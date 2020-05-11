using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.ExpressionExecutors
{
    internal class SymbolExpressionExecutor : IExpressionExecutor
    {
        private readonly Func<Frame, Value> _getter;

        public SymbolExpressionExecutor(Symbol symbol)
        {
            _getter = Frame.CreateGetter(symbol);
        }

        public Value Execute(Frame frame, TextWriter output)
        {
            return _getter(frame);
        }
    }
}