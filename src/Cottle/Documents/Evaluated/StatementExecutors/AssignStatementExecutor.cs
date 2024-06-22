using System;
using System.IO;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors
{
    internal abstract class AssignStatementExecutor : IStatementExecutor
    {
        private readonly Action<Runtime, Frame, Value> _setter;

        protected AssignStatementExecutor(Symbol symbol, StoreMode mode)
        {
            var index = symbol.Index;

            _setter = mode switch
            {
                StoreMode.Global => (state, _, value) => state.GlobalValues[index] = value,
                StoreMode.Local => (_, frame, value) => frame.Locals[index] = value,
                _ => throw new ArgumentOutOfRangeException(nameof(symbol)),
            };
        }

        public Value? Execute(Runtime runtime, Frame frame, TextWriter output)
        {
            _setter(runtime, frame, EvaluateOperand(runtime, frame, output));

            return null;
        }

        protected abstract Value EvaluateOperand(Runtime runtime, Frame frame, TextWriter output);
    }
}