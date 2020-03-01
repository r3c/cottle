using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cottle.Documents.Compiled;
using Cottle.Values;

namespace Cottle.Documents.Evaluated.Executors.Assign
{
    internal class FunctionAssignExecutor : AssignExecutor
    {
        private readonly IReadOnlyList<int> _arguments;

        private readonly IExecutor _body;

        private readonly int _localCount;

        public FunctionAssignExecutor(Symbol symbol, int localCount, IReadOnlyList<int> arguments, IExecutor body) :
            base(symbol)
        {
            _arguments = arguments;
            _body = body;
            _localCount = localCount;
        }

        protected override Value Evaluate(Frame frame, TextWriter output)
        {
            return new FunctionValue(new Function(_localCount, _arguments, _body));
        }

        private class Function : IFunction
        {
            public bool IsPure => false;

            private readonly IReadOnlyList<int> _arguments;

            private readonly IExecutor _body;

            private readonly int _localCount;

            public Function(int localCount, IReadOnlyList<int> arguments, IExecutor body)
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

                var functionArguments = Math.Min(_arguments.Count, arguments.Count);
                var functionFrame = new Frame(parentFrame.Globals, _localCount);

                for (var i = 0; i < functionArguments; ++i)
                    functionFrame.Locals[_arguments[i]] = arguments[i];

                for (var i = arguments.Count; i < _arguments.Count; ++i)
                    functionFrame.Locals[_arguments[i]] = VoidValue.Instance;

                _body.Execute(functionFrame, output, out var result);

                return result;
            }
        }
    }
}