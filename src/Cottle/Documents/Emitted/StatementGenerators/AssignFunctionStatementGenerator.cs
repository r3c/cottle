using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class AssignFunctionStatementGenerator : IStatementGenerator
    {
        private readonly IReadOnlyList<Symbol> _arguments;
        private readonly IStatementGenerator _body;
        private readonly Symbol _symbol;

        public AssignFunctionStatementGenerator(Symbol symbol, IReadOnlyList<Symbol> arguments,
            IStatementGenerator body)
        {
            _arguments = arguments;
            _body = body;
            _symbol = symbol;
        }

        public bool Generate(Emitter emitter)
        {
            var program = Program.Create(_body, _arguments);
            var function = Value.FromFunction(new Function(program));

            switch (_symbol.Mode)
            {
                case StoreMode.Global:
                    emitter.EmitLoadFrameGlobal();
                    emitter.EmitLoadInteger(_symbol.Index);
                    emitter.EmitLoadConstant(function);
                    emitter.EmitStoreElementAtIndex<Value>();

                    break;

                case StoreMode.Local:
                    emitter.EmitLoadConstant(function);
                    emitter.EmitStoreLocal(emitter.GetOrDeclareSymbol(_symbol.Index));

                    break;

                default:
                    throw new InvalidOperationException();
            }

            return false;
        }

        private class Function : IFunction
        {
            public bool IsPure => false;

            private readonly Program _program;

            public Function(Program program)
            {
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
                    throw new InvalidOperationException(
                        $"Invalid function invoke, you seem to have injected a function declared in a {nameof(EmittedDocument)} from another type of document.");

                var functionFrame = parentFrame.CreateForFunction(arguments);

                return _program.Execute(_program.Constants, functionFrame, output, out var result)
                    ? result
                    : Value.Undefined;
            }
        }
    }
}