using System;
using System.Collections.Generic;
using System.IO;
using Cottle.Documents.Compiled;
using Cottle.Documents.Emitted;

namespace Cottle.Documents
{
    internal class EmittedDocument : CompiledDocument<IStatementGenerator, Program>
    {
        public EmittedDocument(Statement statement) :
            base(new Assembler(), generator => Program.Create(generator, Array.Empty<Symbol>()), statement)
        {
        }

        protected override Value Execute(Program program, Value[] globals, int locals, TextWriter writer)
        {
            var frame = new Frame(globals, Array.Empty<Value>(), new Stack<IFunction>());

            return program.Execute(program.Constants, frame, writer, out var result) ? result : Value.Undefined;
        }
    }
}