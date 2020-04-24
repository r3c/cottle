using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Executors
{
    internal abstract class AssignExecutor : IExecutor
    {
        private readonly Action<Frame, Value> _setter;

        protected AssignExecutor(Symbol symbol)
        {
            _setter = Frame.CreateSetter(symbol);
        }

        protected abstract Value Evaluate(Frame frame, TextWriter output);

        public bool Execute(Frame frame, TextWriter output, out Value result)
        {
            _setter(frame, Evaluate(frame, output));

            result = Value.Undefined;

            return false;
        }
    }
}