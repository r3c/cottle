using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal abstract class AssignStatementExecutor : IStatementExecutor
    {
        private readonly Action<Frame, Value> _setter;

        protected AssignStatementExecutor(Symbol symbol)
        {
            _setter = Frame.CreateSetter(symbol);
        }

        public Value? Execute(Frame frame, TextWriter output)
        {
            _setter(frame, EvaluateOperand(frame, output));

            return null;
        }

        protected abstract Value EvaluateOperand(Frame frame, TextWriter output);
    }
}