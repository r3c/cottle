using System;
using System.IO;
using Cottle.Values;

namespace Cottle.Documents.Default.Executors
{
    internal abstract class AssignExecutor : IExecutor
    {
        private readonly int _index;

        private readonly Action<Stack, int, Value> _setter;

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

        protected abstract Value Evaluate(Stack stack, TextWriter output);

        public bool Execute(Stack stack, TextWriter output, out Value result)
        {
            _setter(stack, _index, Evaluate(stack, output));

            result = VoidValue.Instance;

            return false;
        }
    }
}