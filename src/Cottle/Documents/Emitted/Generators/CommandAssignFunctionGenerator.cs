using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cottle.Documents.Compiled;
using Cottle.Values;

namespace Cottle.Documents.Emitted.Generators
{
    internal class CommandAssignFunctionGenerator : IGenerator
    {
        private readonly IReadOnlyList<int> _arguments;
        private readonly IGenerator _body;
        private readonly int _localCount;
        private readonly Symbol _symbol;

        public CommandAssignFunctionGenerator(Symbol symbol, int localCount, IReadOnlyList<int> arguments,
            IGenerator body)
        {
            _arguments = arguments;
            _body = body;
            _localCount = localCount;
            _symbol = symbol;
        }

        public void Generate(Emitter emitter)
        {
            var program = Program.Create(_body);

            emitter.LoadFrameSymbol(_symbol);
            emitter.LoadConstant(new FunctionValue(new Function(_localCount, _arguments, program)));
            emitter.StoreReferenceAtIndex();
            emitter.LoadBoolean(false);
        }

        private class Function : IFunction
        {
            public bool IsPure => false;

            private readonly IReadOnlyList<int> _arguments;
            private readonly int _localCount;
            private readonly Program _program;

            public Function(int localCount, IReadOnlyList<int> arguments, Program program)
            {
                _arguments = arguments;
                _localCount = localCount;
                _program = program;
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
                    throw new InvalidOperationException($"Invalid function invoke, you seem to have injected a function declared in a {nameof(EmittedDocument)} from another type of document.");

                var functionArguments = Math.Min(_arguments.Count, arguments.Count);
                var functionFrame = new Frame(parentFrame.Globals, _localCount);

                for (var i = 0; i < functionArguments; ++i)
                    functionFrame.Locals[_arguments[i]] = arguments[i];

                for (var i = arguments.Count; i < _arguments.Count; ++i)
                    functionFrame.Locals[_arguments[i]] = VoidValue.Instance;

                return _program.Executable(_program.Constants, functionFrame, output, out var result)
                    ? result
                    : VoidValue.Instance;
            }
        }
    }
}