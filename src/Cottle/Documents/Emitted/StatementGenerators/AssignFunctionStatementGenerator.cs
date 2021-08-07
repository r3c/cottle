using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators
{
    internal class AssignFunctionStatementGenerator : IStatementGenerator
    {
        private readonly IStatementGenerator _body;
        private readonly StoreMode _mode;
        private readonly IReadOnlyList<Symbol> _slots;
        private readonly Symbol _symbol;

        public AssignFunctionStatementGenerator(Symbol symbol, StoreMode mode, IReadOnlyList<Symbol> slots, IStatementGenerator body)
        {
            _body = body;
            _mode = mode;
            _slots = slots;
            _symbol = symbol;
        }

        public bool Generate(Emitter emitter)
        {
            var program = Program.Create(_body, _slots);
            var function = Value.FromFunction(new Function(program));

            switch (_mode)
            {
                case StoreMode.Global:
                    emitter.EmitLoadFrameGlobal();
                    emitter.EmitLoadInteger(_symbol.Index);
                    emitter.EmitLoadConstant(function);
                    emitter.EmitStoreElementAtIndex<Value>();

                    break;

                case StoreMode.Local:
                    emitter.EmitLoadConstant(function);
                    emitter.EmitStoreLocal(emitter.GetOrDeclareLocal(_symbol));

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
                if (state is not Frame parentFrame)
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