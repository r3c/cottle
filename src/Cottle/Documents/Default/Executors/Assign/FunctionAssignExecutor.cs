using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Cottle.Values;

namespace Cottle.Documents.Default.Executors.Assign
{
    internal class FunctionAssignExecutor : AssignExecutor
    {
        private readonly int[] _arguments;

        private readonly IExecutor _body;

        private readonly int _localCount;

        public FunctionAssignExecutor(Symbol symbol, int localCount, IEnumerable<int> arguments, IExecutor body) :
            base(symbol)
        {
            _arguments = arguments.ToArray();
            _body = body;
            _localCount = localCount;
        }

        protected override Value Evaluate(Frame frame, TextWriter output)
        {
            return new FunctionValue(new NodeFunction(_localCount, _arguments, _body));
        }

        private class NodeFunction : IFunction
        {
            public bool IsPure => false;

            private readonly int[] _arguments;

            private readonly IExecutor _body;

            private readonly int _localCount;

            public NodeFunction(int localCount, int[] arguments, IExecutor body)
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

            public Value Invoke(object state, IReadOnlyList<Value> arguments, TextWriter output)
            {
                if (!(state is Frame parentStack))
                    throw new InvalidOperationException($"Invalid function invoke, you seem to have injected a function declared in a {nameof(DefaultDocument)} from another type of document.");

                var functionArguments = Math.Min(_arguments.Length, arguments.Count);
                var functionStack = new Frame(parentStack.Globals, _localCount);

                for (var i = 0; i < functionArguments; ++i)
                    functionStack.Locals[_arguments[i]] = arguments[i];

                for (var i = arguments.Count; i < _arguments.Length; ++i)
                    functionStack.Locals[_arguments[i]] = VoidValue.Instance;

                _body.Execute(functionStack, output, out var result);

                return result;
            }

            public override int GetHashCode()
            {
                return RuntimeHelpers.GetHashCode(this);
            }
        }
    }
}