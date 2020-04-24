using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Evaluators
{
    internal class SymbolEvaluator : IEvaluator
    {
        private readonly Func<Frame, Value> _getter;

        public SymbolEvaluator(Symbol symbol)
        {
            _getter = Frame.CreateGetter(symbol);
        }

        public Value Evaluate(Frame frame, TextWriter output)
        {
            return _getter(frame);
        }
    }
}