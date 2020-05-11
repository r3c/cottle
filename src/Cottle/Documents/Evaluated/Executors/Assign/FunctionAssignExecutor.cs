using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Evaluated.Executors.Assign
{
    internal class FunctionAssignExecutor : AssignExecutor
    {
        private readonly Value _function;

        public FunctionAssignExecutor(Symbol symbol, int localCount, IReadOnlyList<Symbol> arguments, IExecutor body) :
            base(symbol)
        {
            _function = Value.FromFunction(new Function(localCount, arguments, body));
        }

        protected override Value Evaluate(Frame frame, TextWriter output)
        {
            return _function;
        }

        private class Function : IFunction
        {
            public bool IsPure => false;

            private readonly IReadOnlyList<Symbol> _arguments;

            private readonly IExecutor _body;

            private readonly int _localCount;

            public Function(int localCount, IReadOnlyList<Symbol> arguments, IExecutor body)
            {
                _arguments = arguments;
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

                var functionFrame = parentFrame.CreateForFunction(_arguments, arguments, _localCount);

                _body.Execute(functionFrame, output, out var result);

                return result;
            }
        }
    }
}