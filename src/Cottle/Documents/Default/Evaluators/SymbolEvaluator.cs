using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Default.Evaluators
{
    internal class SymbolEvaluator : IEvaluator
    {
        private readonly Func<Frame, int, Value> _getter;

        private readonly int _index;

        public SymbolEvaluator(Symbol symbol)
        {
            switch (symbol.Mode)
            {
                case StoreMode.Global:
                    _getter = (stack, index) => stack.Globals[index];

                    break;

                case StoreMode.Local:
                    _getter = (stack, index) => stack.Locals[index] ?? VoidValue.Instance;

                    break;

                default:
                    throw new NotImplementedException();
            }

            _index = symbol.Index;
        }

        public Value Evaluate(Frame frame, TextWriter output)
        {
            return _getter(frame, _index);
        }
    }
}