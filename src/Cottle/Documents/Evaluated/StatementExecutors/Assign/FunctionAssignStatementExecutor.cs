using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.StatementExecutors.Assign
{
    internal class FunctionAssignStatementExecutor : AssignStatementExecutor
    {
        private readonly Value _function;

        public FunctionAssignStatementExecutor(Symbol symbol, int localCount, IReadOnlyList<Symbol> arguments, IStatementExecutor body) :
            base(symbol)
        {
            _function = Value.FromFunction(new Function(localCount, arguments, body));
        }

        protected override Value EvaluateOperand(Frame frame, TextWriter output)
        {
            return _function;
        }

        private class Function : IFunction
        {
            public bool IsPure => false;

            private readonly IReadOnlyList<Action<Frame, Value>> _argumentSetters;

            private readonly IStatementExecutor _body;

            private readonly int _localCount;

            public Function(int localCount, IReadOnlyList<Symbol> arguments, IStatementExecutor body)
            {
                _argumentSetters = arguments.Select(Frame.CreateSetter).ToList();
                _body = body;
                _localCount = localCount;
            }

            public int CompareTo(IFunction other)
            {
                return object.ReferenceEquals(this, other) ? 0 : 1;
            }

            public bool Equals(IFunction other)
            {
                return CompareTo(other) == 0;
            }

            public override bool Equals(object obj)
            {
                return obj is IFunction other && Equals(other);
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(this);
            }

            public Value Invoke(object state, IReadOnlyList<Value> arguments, TextWriter output)
            {
                if (!(state is Frame parentFrame))
                    throw new InvalidOperationException($"Invalid function invoke, you seem to have injected a function declared in a {nameof(EvaluatedDocument)} from another type of document.");

                var functionFrame = parentFrame.CreateForFunction(_argumentSetters, arguments, _localCount);

                return _body.Execute(functionFrame, output).GetValueOrDefault(Value.Undefined);
            }
        }
    }
}