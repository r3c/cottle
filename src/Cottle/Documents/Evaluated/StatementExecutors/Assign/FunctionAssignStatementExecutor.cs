using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors.Assign
{
    internal class FunctionAssignStatementExecutor : AssignStatementExecutor
    {
        private readonly Value _function;

        public FunctionAssignStatementExecutor(Symbol symbol, StoreMode mode, int localCount, IReadOnlyList<Symbol> slots, IStatementExecutor body) :
            base(symbol, mode)
        {
            _function = Value.FromFunction(new Function(localCount, slots, body));
        }

        protected override Value EvaluateOperand(Frame frame, TextWriter output)
        {
            return _function;
        }

        private class Function : IFunction
        {
            public bool IsPure => false;

            private readonly IStatementExecutor _body;

            private readonly int _localCount;

            private readonly IReadOnlyList<Symbol> _slots;

            public Function(int localCount, IReadOnlyList<Symbol> slots, IStatementExecutor body)
            {
                _body = body;
                _localCount = localCount;
                _slots = slots;
            }

            public int CompareTo(IFunction? other)
            {
                return object.ReferenceEquals(this, other) ? 0 : 1;
            }

            public bool Equals(IFunction? other)
            {
                return CompareTo(other) == 0;
            }

            public override bool Equals(object? obj)
            {
                return obj is IFunction other && Equals(other);
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(this);
            }

            public Value Invoke(object state, IReadOnlyList<Value> arguments, TextWriter output)
            {
                if (state is not Frame parentFrame)
                    throw new InvalidOperationException(
                        $"Invalid function invoke, you seem to have injected a function declared in a {nameof(EvaluatedDocument)} from another type of document.");

                var functionArguments = Math.Min(_slots.Count, arguments.Count);
                var functionFrame = parentFrame.CreateForFunction(_localCount);

                for (var i = 0; i < functionArguments; ++i)
                    functionFrame.Locals[_slots[i].Index] = arguments[i];

                for (var i = arguments.Count; i < _slots.Count; ++i)
                    functionFrame.Locals[_slots[i].Index] = Value.Undefined;

                return _body.Execute(functionFrame, output).GetValueOrDefault(Value.Undefined);
            }
        }
    }
}