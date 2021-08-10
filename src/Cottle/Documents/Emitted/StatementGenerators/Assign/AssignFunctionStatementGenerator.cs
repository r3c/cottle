using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Cottle.Documents.Compiled;

namespace Cottle.Documents.Emitted.StatementGenerators.Assign
{
    internal class FunctionAssignStatementGenerator : AssignStatementGenerator
    {
        private readonly IStatementGenerator _body;
        private readonly IReadOnlyList<Symbol> _slots;

        public FunctionAssignStatementGenerator(Symbol symbol, StoreMode mode, IReadOnlyList<Symbol> slots, IStatementGenerator body) :
            base(symbol, mode)
        {
            _body = body;
            _slots = slots;
        }

        public override bool Generate(Emitter emitter)
        {
            var program = Program.Create(_body, _slots);

            emitter.EmitLoadConstant(Value.FromFunction(new Function(program)));

            GenerateAssignment(emitter);

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

                return _program.Execute(functionFrame, output);
            }
        }
    }
}