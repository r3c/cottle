using System;
using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Evaluated;
using Cottle.Values;

namespace Cottle.Documents.Evaluated.Executors
{
    internal abstract class AssignExecutor : IExecutor
    {
        private readonly int _index;

        private readonly Action<Frame, int, Value> _setter;

        protected AssignExecutor(Symbol symbol)
        {
            switch (symbol.Mode)
            {
                case StoreMode.Global:
                    _setter = (stack, index, value) => stack.Globals[index] = value;

                    break;

                case StoreMode.Local:
                    _setter = (stack, index, value) => stack.Locals[index] = value;

                    break;

                default:
                    throw new NotImplementedException();
            }

            _index = symbol.Index;
        }

        protected abstract Value Evaluate(Frame frame, TextWriter output);

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            _setter(frame, _index, Evaluate(frame, output));

            result = VoidValue.Instance;

            return false;
        }
    }
}